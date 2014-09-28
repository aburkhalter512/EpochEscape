using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;

public class SceneManager : Manager<SceneManager>
{
    private GameObject m_loadingAnimation = null;
    private SpriteRenderer m_loadingAnimationRenderer = null;

    private Color m_defaultAlpha = new Color(1f, 1f, 1f, 1f);
    private Color m_transparent = new Color(1f, 1f, 1f, 0f);
    private Color m_currentAlpha = new Color(1f, 1f, 1f, 1f);

    public string m_loadingMessage = "Loading (%p)";
    public bool m_fading = false;
    public bool m_slowSimulation = false;
    public float m_simulationTime = 1f;
    private int m_numberOfObjectsToLoad = 0;
    public GUIStyle m_style = null;

    private int m_objectsLoaded = 0;
    private bool m_isLoading = false;
    private string loadingText = string.Empty;
    private List<string> m_assets = null;
    private string m_currentAsset = string.Empty;

    private float percentage = 0f;

    // ---

    private List<string> m_levels = null;
    private int m_currentLevel = 0;
    private string m_currentLevelName = string.Empty;

    protected override void Initialize()
    {
        _CreateLevelList();
        _CreateLoadingAnimation();
        _StartLoading();
    }

    private void _CreateLevelList()
    {
        TextAsset levels = Resources.Load("Data/Levels/levels") as TextAsset;

        if (levels != null)
        {
            Dictionary<string, object> levelsJSON = Json.Deserialize(levels.text) as Dictionary<string, object>;
            List<object> levelsTemp = null;

            if (levelsJSON != null)
            {
                levelsTemp = levelsJSON["levels"] as List<object>;

                if (levelsTemp != null)
                {
                    m_levels = new List<string>();

                    foreach (object level in levelsTemp)
                        m_levels.Add(level.ToString());
                }
            }
        }
    }

    private void _CreateLoadingAnimation()
    {
        m_loadingAnimation = Resources.Load("Prefabs/GUI/LoadingAnimation") as GameObject;

        if (m_loadingAnimation != null)
        {
            m_loadingAnimation = Instantiate(m_loadingAnimation) as GameObject;

            if (m_loadingAnimation != null)
            {
                m_loadingAnimationRenderer = m_loadingAnimation.GetComponent<SpriteRenderer>();

                DontDestroyOnLoad(m_loadingAnimation);

                m_loadingAnimation.SetActive(false);
            }
        }
    }

    private void _StartLoading()
    {
        m_isLoading = true;
        m_loadingAnimation.SetActive(true);

        _ClearLevel();

        StartCoroutine(_Load());
    }

    private IEnumerator _Load()
    {
        GameManager.getInstance().PauseMovement();

        Debug.Log("Attempting to load " + m_levels[m_currentLevel]);

        TextAsset levelDataText = Resources.Load("Data/Levels/" + m_levels[m_currentLevel]) as TextAsset;

        if (levelDataText != null)
        {
            string levelDataJSON = levelDataText.text;

            // Deserialize the level data, and convert it to a dictionary of keys and values.
            Dictionary<string, object> levelData = Json.Deserialize(levelDataJSON) as Dictionary<string, object>;

            yield return StartCoroutine(ConstructLevel(levelData));
            yield return StartCoroutine(ConstructChambers(levelData));
            yield return StartCoroutine(OnFinishLoad());
        }

        GameManager.getInstance().UnpauseMovement();
    }

    private IEnumerator OnFinishLoad()
    {
        yield return new WaitForSeconds(0.5f);

        LevelManager.Ready();

        if(m_loadingAnimation != null)
        {
            if (m_loadingAnimationRenderer != null && m_fading)
            {
                while (m_loadingAnimationRenderer.color.a > 0f)
                {
                    m_currentAlpha = Color.Lerp(m_loadingAnimationRenderer.color, m_transparent, 5f * Time.smoothDeltaTime);
                    m_style.normal.textColor = m_currentAlpha;
                    m_loadingAnimationRenderer.color = m_currentAlpha;

                    if(Utilities.IsApproximately(m_loadingAnimationRenderer.color.a, 0f, 0.01f))
                        m_loadingAnimationRenderer.color = m_transparent;

                    yield return null;
                }
            }

            m_style.normal.textColor = m_defaultAlpha;
            m_loadingAnimationRenderer.color = m_defaultAlpha;
            m_loadingAnimation.SetActive(false);
        }

        m_isLoading = false;
    }

    private IEnumerator ConstructLevel(Dictionary<string, object> levelData)
    {
        Dictionary<string, object> levelDict = levelData["level"] as Dictionary<string, object>;

        if (levelDict != null && levelDict.Count > 0)
        {
            m_currentLevelName = levelDict["name"] as string;
            m_numberOfObjectsToLoad = (int)((long)levelDict["numberOfObjects"]);
        }

        yield break;
    }

    private IEnumerator ConstructChambers(Dictionary<string, object> levelData)
    {
        List<object> chambers = levelData["chambers"] as List<object>;
        Dictionary<string, object> chamberData = null;

        for (int i = 0; i < chambers.Count; i++)
        {
            chamberData = chambers[i] as Dictionary<string, object>;
            
            yield return StartCoroutine(ConstructChamber(chamberData));
        }
    }

    private IEnumerator ConstructChamber(Dictionary<string, object> chamberDict)
    {
        if (chamberDict != null)
        {
            string chamberName = chamberDict["name"] as string;
            string chamberPosition = chamberDict["position"] as string;
            string chamberSize = chamberDict["size"] as string;

            Chamber chamber = new Chamber();

            if (chamber != null)
            {
                chamber.SetName(chamberName);
                chamber.SetPosition(LevelEditorUtilities.StringToVector3(chamberPosition));
                chamber.SetSize(LevelEditorUtilities.StringToVector2(chamberSize));

                LevelManager.AddChamber(chamber);

                yield return StartCoroutine(ConstructChunks(chamberDict, chamber));
                yield return StartCoroutine(ConstructDoors(chamberDict, chamber));
                yield return StartCoroutine(ConstructItems(chamberDict, chamber));
            }
        }
    }

    private IEnumerator ConstructChunks(Dictionary<string, object> chamberDict, Chamber chamber)
    {
        if (chamberDict != null)
        {
            List<object> chunks = chamberDict["chunks"] as List<object>;
            Dictionary<string, object> chunkDict = null;

            GameObject chunksContainer = new GameObject();

            if (chunksContainer != null)
            {
                chamber.SetChild(chunksContainer);

                chunksContainer.name = "Chunks";
                chunksContainer.transform.localPosition = Vector3.zero;

                for (int i = 0; i < chunks.Count; i++)
                {
                    chunkDict = chunks[i] as Dictionary<string, object>;

                    yield return StartCoroutine(ConstructChunk(chunkDict, chunksContainer));
                }
            }
        }
    }

    private IEnumerator ConstructChunk(Dictionary<string, object> chunkDict, GameObject parent)
    {
        if (chunkDict != null)
        {
            string chunkName = chunkDict["name"] as string;
            string chunkPosition = chunkDict["position"] as string;

            GameObject chunk = new GameObject();

            if (chunk != null)
            {
                SpriteRenderer renderer = chunk.AddComponent<SpriteRenderer>();

                if (renderer != null)
                {
                    renderer.sprite = Resources.Load<Sprite>("Textures/Levels/" + m_currentLevelName + "/Chamber" + Chamber.GetChamberCount() + "/" + chunkName);

                    if(renderer.sprite == null)
                        Debug.Log("Textures/Levels/" + m_currentLevelName + "/Chamber" + Chamber.GetChamberCount() + "/" + chunkName);

                    chunk.name = chunkName;
                    chunk.transform.parent = parent.transform;
                    chunk.transform.localPosition = LevelEditorUtilities.StringToVector3(chunkPosition);

                    m_objectsLoaded++;

                    if (m_slowSimulation)
                        yield return new WaitForSeconds(m_simulationTime);
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructDoors(Dictionary<string, object> chamberDict, Chamber chamber)
    {
        if (chamberDict != null)
        {
            List<object> doors = chamberDict["doors"] as List<object>;
            Dictionary<string, object> doorDict = null;

            GameObject doorsContainer = new GameObject();

            if (doorsContainer != null)
            {
                chamber.SetChild(doorsContainer);

                doorsContainer.name = "Doors";
                doorsContainer.transform.localPosition = Vector3.zero;

                for (int i = 0; i < doors.Count; i++)
                {
                    doorDict = doors[i] as Dictionary<string, object>;

                    yield return StartCoroutine(ConstructDoor(doorDict, doorsContainer));
                }
            }
        }
    }

    private IEnumerator ConstructDoor(Dictionary<string, object> doorDict, GameObject parent)
    {
        if (doorDict != null)
        {
            string doorName = doorDict["name"] as string;
            //string doorPath = doorDict["path"] as string;
            string position = doorDict["position"] as string;
            string direction = doorDict["direction"] as string;

            GameObject door = Resources.Load("Prefabs/Doors/Combos/" + doorName) as GameObject;

            if (door != null)
            {
                door = Instantiate(door) as GameObject;

                if (door != null)
                {
                    ISerializable doorSerializer = door.GetComponent(typeof(ISerializable)) as ISerializable;

                    door.name = doorName;
                    door.transform.parent = parent.transform;
                    door.transform.localPosition = LevelEditorUtilities.StringToVector3(position);
                    door.transform.up = LevelEditorUtilities.StringToVector3(direction);

                    if (doorSerializer != null)
                        doorSerializer.Unserialize(ref doorDict);

                    m_objectsLoaded++;

                    if (m_slowSimulation)
                        yield return new WaitForSeconds(m_simulationTime);
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructItems(Dictionary<string, object> chamberDict, Chamber chamber)
    {
        if (chamberDict != null)
        {
            Dictionary<string, object> itemsDict = chamberDict["items"] as Dictionary<string, object>;

            GameObject itemsContainer = new GameObject();

            if (itemsContainer != null)
            {
                chamber.SetChild(itemsContainer);

                itemsContainer.name = "Items";
                itemsContainer.transform.localPosition = Vector3.zero;
                
                yield return StartCoroutine(ConstructPowerCores(itemsDict, itemsContainer));
                yield return StartCoroutine(ConstructCrates(itemsDict, itemsContainer));
            }
        }
    }

    private IEnumerator ConstructPowerCores(Dictionary<string, object> itemsDict, GameObject parent)
    {
        if (itemsDict != null)
        {
            List<object> powerCores = itemsDict["powerCores"] as List<object>;
            Dictionary<string, object> powerCoreDict = null;

            GameObject powerCoresContainer = new GameObject();

            if (powerCoresContainer != null)
            {
                powerCoresContainer.name = "Cores";
                powerCoresContainer.transform.parent = parent.transform;
                powerCoresContainer.transform.localPosition = Vector3.zero;

                for (int i = 0; i < powerCores.Count; i++)
                {
                    powerCoreDict = powerCores[i] as Dictionary<string, object>;

                    yield return StartCoroutine(ConstructPowerCore(powerCoreDict, powerCoresContainer));
                }
            }
        }
    }

    private IEnumerator ConstructPowerCore(Dictionary<string, object> powerCoreDict, GameObject parent)
    {
        if (powerCoreDict != null)
        {
            string powerCoreName = powerCoreDict["name"] as string;
            string powerCorePosition = powerCoreDict["position"] as string;
            string powerCoreDirection = powerCoreDict["direction"] as string;

            GameObject powerCore = Resources.Load("Prefabs/Items/" + powerCoreName) as GameObject;

            if (powerCore != null)
            {
                powerCore = Instantiate(powerCore) as GameObject;

                if (powerCore != null)
                {
                    powerCore.name = powerCoreName;
                    powerCore.transform.parent = parent.transform;
                    powerCore.transform.localPosition = LevelEditorUtilities.StringToVector3(powerCorePosition);
                    powerCore.transform.up = LevelEditorUtilities.StringToVector3(powerCoreDirection);

                    m_objectsLoaded++;

                    if (m_slowSimulation)
                        yield return new WaitForSeconds(m_simulationTime);
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructCrates(Dictionary<string, object> itemsDict, GameObject parent)
    {
        if (itemsDict != null)
        {
            List<object> crates = itemsDict["crates"] as List<object>;
            Dictionary<string, object> crateDict = null;

            GameObject cratesContainer = new GameObject();

            if (cratesContainer != null)
            {
                cratesContainer.name = "Crates";
                cratesContainer.transform.parent = parent.transform;
                cratesContainer.transform.localPosition = Vector3.zero;

                for (int i = 0; i < crates.Count; i++)
                {
                    crateDict = crates[i] as Dictionary<string, object>;

                    yield return StartCoroutine(ConstructCrate(crateDict, cratesContainer));
                }
            }
        }
    }

    private IEnumerator ConstructCrate(Dictionary<string, object> crateDict, GameObject parent)
    {
        if (crateDict != null)
        {
            string crateName = crateDict["name"] as string;

            GameObject crate = Resources.Load("Prefabs/Obstacles/" + crateName) as GameObject;

            if (crate != null)
            {
                crate = Instantiate(crate) as GameObject;

                if (crate != null)
                {
                    ISerializable crateSerializer = crate.GetComponent(typeof(ISerializable)) as ISerializable;

                    /*
                    if (crateSerializer != null)
                        crateSerializer.Unserialize(crateDict);*/

                    crate.name = crateName;
                    crate.transform.parent = parent.transform;

                    m_objectsLoaded++;

                    if (m_slowSimulation)
                        yield return new WaitForSeconds(m_simulationTime);
                }
            }
        }

        yield break;
    }

    private void _LoadNextLevel()
    {
        m_currentLevel++;

        if (m_currentLevel >= m_levels.Count)
            m_currentLevel = 0;

        _StartLoading();
    }

    private void _ClearLevel()
    {
        LevelManager.ClearLevel();

        m_objectsLoaded = 0;
    }

    public void OnGUI()
    {
        if (m_isLoading)
        {
            if (m_numberOfObjectsToLoad > 0)
                percentage = Mathf.Floor(((float)m_objectsLoaded / m_numberOfObjectsToLoad) * 100f);

            loadingText = m_loadingMessage.Replace("%p", percentage.ToString() + "%");

            GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height - 50f), loadingText, m_style);
        }
    }

    public static void Load(string levelName)
    {
        //SceneManager.GetInstance()._StartLoading(levelName);
    }

    public static void LoadNextLevel()
    {
        SceneManager.GetInstance()._LoadNextLevel();
    }
}
