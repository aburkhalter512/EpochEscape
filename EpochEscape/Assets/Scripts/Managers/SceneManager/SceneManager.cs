/* Things to do
 * 
 * 1) Discover a recursive solution to the object generation since code is all too similar.
 */

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
    public GUIStyle m_style = null;

    private int m_objectsLoaded = 0;
    private int m_objectsToLoad = 0;
    private bool m_isLoading = false;
    private string m_loadingText = string.Empty;

    private float m_loadingPercentage = 0f;

    // ---

    private List<string> m_levels = null;
    private int m_currentLevel = 0;
    private string m_currentLevelName = string.Empty;

    // ---

    private List<GameObject> m_doors = null;
    private List<GameObject> m_dynamicWalls = null;

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
        GameManager.Get().PauseMovement();

        Debug.Log("Attempting to load " + m_levels[m_currentLevel] + ".");

        TextAsset levelDataText = Resources.Load("Data/Levels/" + m_levels[m_currentLevel]) as TextAsset;

        if (levelDataText != null)
        {
            string levelDataJSON = levelDataText.text;

            Dictionary<string, object> levelData = Json.Deserialize(levelDataJSON) as Dictionary<string, object>;

            yield return StartCoroutine(ConstructLevel(levelData));
            yield return StartCoroutine(ConstructChambers(levelData));
            yield return StartCoroutine(ConstructHalls(levelData));
            yield return StartCoroutine(OnFinishLoad());
        }

        GameManager.Get().UnpauseMovement();
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

        Debug.Log("Finished loading level.");
    }

    private IEnumerator ConstructLevel(Dictionary<string, object> levelData)
    {
        if (levelData != null && levelData.ContainsKey("level"))
        {
            Dictionary<string, object> levelDict = levelData["level"] as Dictionary<string, object>;

            if (levelDict != null && levelDict.Count > 0)
            {
                m_currentLevelName = levelDict["name"] as string;
                m_objectsToLoad = (int)((long)levelDict["numberOfObjects"]);
            }
        }

        yield break;
    }

    private IEnumerator ConstructChambers(Dictionary<string, object> levelData)
    {
        List<object> chambers = levelData["chambers"] as List<object>;
        Dictionary<string, object> chamberData = null;

        if (levelData != null)
        {
            for (int i = 0; i < chambers.Count; i++)
            {
                chamberData = chambers[i] as Dictionary<string, object>;

                if(chamberData != null)
                    yield return StartCoroutine(ConstructChamber(chamberData));
            }
        }

        yield break;
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
                yield return StartCoroutine(ConstructDynamicWalls(chamberDict, chamber));
                yield return StartCoroutine(ConstructTriggers(chamberDict, chamber));
                yield return StartCoroutine(ConstructItems(chamberDict, chamber));
                yield return StartCoroutine(ConstructWallColliders(chamberDict, chamber));
            }
        }

        yield break;
    }

    private IEnumerator ConstructHalls(Dictionary<string, object> levelData)
    {
        List<object> halls = levelData["halls"] as List<object>;
        Dictionary<string, object> hallData = null;

        if (levelData != null)
        {
            for (int i = 0; i < halls.Count; i++)
            {
                hallData = halls[i] as Dictionary<string, object>;

                if (hallData != null)
                    yield return StartCoroutine(ConstructHall(hallData));
            }
        }

        yield break;
    }

    private IEnumerator ConstructHall(Dictionary<string, object> hallDict)
    {
        if (hallDict != null)
        {
            string hallName = hallDict["name"] as string;
            string hallPosition = hallDict["position"] as string;
            string hallSize = hallDict["size"] as string;

            Hall hall = new Hall();

            if (hall != null)
            {
                hall.SetName(hallName);
                hall.SetPosition(LevelEditorUtilities.StringToVector3(hallPosition));
                hall.SetSize(LevelEditorUtilities.StringToVector2(hallSize));

                yield return StartCoroutine(ConstructChunks(hallDict, hall));
                yield return StartCoroutine(ConstructWallColliders(hallDict, hall));
            }
        }

        yield break;
    }

    private IEnumerator ConstructChunks(Dictionary<string, object> chamberDict, Room room)
    {
        if (chamberDict != null && chamberDict.ContainsKey("chunks") && room != null)
        {
            List<object> chunks = chamberDict["chunks"] as List<object>;
            Dictionary<string, object> chunkDict = null;

            GameObject chunksContainer = new GameObject();

            if (chunks != null && chunksContainer != null)
            {
                room.SetChild(chunksContainer);

                chunksContainer.name = "Chunks";
                chunksContainer.transform.localPosition = Vector3.zero;

                for (int i = 0; i < chunks.Count; i++)
                {
                    chunkDict = chunks[i] as Dictionary<string, object>;

                    if(chunkDict != null)
                        yield return StartCoroutine(ConstructChunk(chunkDict, chunksContainer, room));
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructChunk(Dictionary<string, object> chunkDict, GameObject parent, Room room)
    {
        if (chunkDict != null && parent != null)
        {
            string chunkName = chunkDict["name"] as string;
            string chunkPosition = chunkDict["position"] as string;

            GameObject chunk = new GameObject();

            if (chunk != null)
            {
                SpriteRenderer renderer = chunk.AddComponent<SpriteRenderer>();

                if (renderer != null)
                {
                    renderer.sprite = Resources.Load<Sprite>("Textures/Levels/" + m_currentLevelName + "/" + room.GetRoomType() + room.GetRoomCount() + "/" + chunkName);

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

    private IEnumerator ConstructDoors(Dictionary<string, object> chamberDict, Room room)
    {
        if (chamberDict != null && chamberDict.ContainsKey("doors") && room != null)
        {
            List<object> doors = chamberDict["doors"] as List<object>;
            Dictionary<string, object> doorDict = null;

            GameObject doorsContainer = new GameObject();

            if (doors != null && doorsContainer != null)
            {
                m_doors = new List<GameObject>();

                room.SetChild(doorsContainer);

                doorsContainer.name = "Doors";
                doorsContainer.transform.localPosition = Vector3.zero;

                for (int i = 0; i < doors.Count; i++)
                {
                    doorDict = doors[i] as Dictionary<string, object>;

                    if(doorDict != null)
                        yield return StartCoroutine(ConstructDoor(doorDict, doorsContainer));
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructDoor(Dictionary<string, object> doorDict, GameObject parent)
    {
        if (doorDict != null && parent != null)
        {
            string doorName = doorDict["name"] as string;
            string doorPosition = doorDict["position"] as string;
            string doorDirection = doorDict["direction"] as string;

            GameObject door = Resources.Load("Prefabs/Doors/Combos/" + doorName) as GameObject;

            if (door != null)
            {
                door = Instantiate(door) as GameObject;

                if (door != null)
                {
                    ISerializable doorSerializer = door.GetComponent(typeof(ISerializable)) as ISerializable;

                    door.name = doorName;
                    door.transform.parent = parent.transform;
                    door.transform.localPosition = LevelEditorUtilities.StringToVector3(doorPosition);
                    door.transform.up = LevelEditorUtilities.StringToVector3(doorDirection);

                    m_doors.Add(door);

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

    private IEnumerator ConstructDynamicWalls(Dictionary<string, object> chamberDict, Room room)
    {
        if (chamberDict != null && chamberDict.ContainsKey("dynamicWalls") && room != null)
        {
            List<object> dynamicWalls = chamberDict["dynamicWalls"] as List<object>;
            Dictionary<string, object> dynamicWallDict = null;

            GameObject dynamicWallsContainer = new GameObject();

            if (dynamicWalls != null && dynamicWallsContainer != null)
            {
                m_dynamicWalls = new List<GameObject>();

                room.SetChild(dynamicWallsContainer);

                dynamicWallsContainer.name = "DynamicWalls";
                dynamicWallsContainer.transform.localPosition = Vector3.zero;

                for (int i = 0; i < dynamicWalls.Count; i++)
                {
                    dynamicWallDict = dynamicWalls[i] as Dictionary<string, object>;

                    if (dynamicWallDict != null)
                        yield return StartCoroutine(ConstructDynamicWall(dynamicWallDict, dynamicWallsContainer));
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructDynamicWall(Dictionary<string, object> dynamicWallDict, GameObject parent)
    {
        if (dynamicWallDict != null && parent != null)
        {
            string dynamicWallName = dynamicWallDict["name"] as string;
            string dynamicWallPosition = dynamicWallDict["position"] as string;
            string dynamicWallDirection = dynamicWallDict["direction"] as string;

            GameObject dynamicWall = Resources.Load("Prefabs/Walls/" + dynamicWallName) as GameObject;

            if (dynamicWall != null)
            {
                dynamicWall = Instantiate(dynamicWall) as GameObject;

                if (dynamicWall != null)
                {
                    ISerializable dynamicWallSerializer = dynamicWall.GetComponent(typeof(ISerializable)) as ISerializable;

                    dynamicWall.name = dynamicWallName;
                    dynamicWall.transform.parent = parent.transform;
                    dynamicWall.transform.localPosition = LevelEditorUtilities.StringToVector3(dynamicWallPosition);
                    dynamicWall.transform.up = LevelEditorUtilities.StringToVector3(dynamicWallDirection);

                    m_dynamicWalls.Add(dynamicWall);

                    if (dynamicWallSerializer != null)
                        dynamicWallSerializer.Unserialize(ref dynamicWallDict);

                    m_objectsLoaded++;

                    if (m_slowSimulation)
                        yield return new WaitForSeconds(m_simulationTime);
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructTriggers(Dictionary<string, object> chamberDict, Room room)
    {
        if (chamberDict != null && chamberDict.ContainsKey("triggers") && room != null)
        {
            Dictionary<string, object> triggersDict = chamberDict["triggers"] as Dictionary<string, object>;

            GameObject triggersContainer = new GameObject();

            if (triggersDict != null && triggersContainer != null)
            {
                room.SetChild(triggersContainer);

                triggersContainer.name = "Triggers";
                triggersContainer.transform.localPosition = Vector3.zero;

                yield return StartCoroutine(ConstructTerminals(triggersDict, triggersContainer));
                yield return StartCoroutine(ConstructPlates(triggersDict, triggersContainer));
            }
        }

        yield break;
    }

    private IEnumerator ConstructTerminals(Dictionary<string, object> triggersDict, GameObject parent)
    {
        if (triggersDict != null && triggersDict.ContainsKey("terminals") && parent != null)
        {
            List<object> terminals = triggersDict["terminals"] as List<object>;
            Dictionary<string, object> terminalDict = null;

            GameObject terminalsContainer = new GameObject();

            if (terminals != null && terminalsContainer != null)
            {
                terminalsContainer.name = "Terminals";
                terminalsContainer.transform.parent = parent.transform;
                terminalsContainer.transform.localPosition = Vector3.zero;

                for (int i = 0; i < terminals.Count; i++)
                {
                    terminalDict = terminals[i] as Dictionary<string, object>;

                    if (terminalDict != null)
                        yield return StartCoroutine(ConstructTerminal(terminalDict, terminalsContainer));
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructTerminal(Dictionary<string, object> triggerDict, GameObject parent)
    {
        if (triggerDict != null && parent != null)
        {
            string terminalName = triggerDict["name"] as string;
            string terminalPosition = triggerDict["position"] as string;
            string terminalDirection = triggerDict["direction"] as string;

            GameObject terminal = Resources.Load("Prefabs/Interactors/" + terminalName) as GameObject;

            if (terminal != null)
            {
                terminal = Instantiate(terminal) as GameObject;

                if (terminal != null)
                {
                    ISerializable terminalSerializer = terminal.GetComponent(typeof(ISerializable)) as ISerializable;

                    terminal.name = terminalName;
                    terminal.transform.parent = parent.transform;
                    terminal.transform.localPosition = LevelEditorUtilities.StringToVector3(terminalPosition);
                    terminal.transform.up = LevelEditorUtilities.StringToVector3(terminalDirection);

                    if (terminalSerializer != null)
                        terminalSerializer.Unserialize(ref triggerDict);

                    m_objectsLoaded++;

                    if (m_slowSimulation)
                        yield return new WaitForSeconds(m_simulationTime);
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructPlates(Dictionary<string, object> triggersDict, GameObject parent)
    {
        if (triggersDict != null && triggersDict.ContainsKey("plates") && parent != null)
        {
            List<object> plates = triggersDict["plates"] as List<object>;
            Dictionary<string, object> plateDict = null;

            GameObject platesContainer = new GameObject();

            if (plates != null && platesContainer != null)
            {
                platesContainer.name = "Plates";
                platesContainer.transform.parent = parent.transform;
                platesContainer.transform.localPosition = Vector3.zero;

                for (int i = 0; i < plates.Count; i++)
                {
                    plateDict = plates[i] as Dictionary<string, object>;

                    if (plateDict != null)
                        yield return StartCoroutine(ConstructPlate(plateDict, platesContainer));
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructPlate(Dictionary<string, object> plateDict, GameObject parent)
    {
        if (plateDict != null && parent != null)
        {
            string plateName = plateDict["name"] as string;
            string platePosition = plateDict["position"] as string;
            string plateDirection = plateDict["direction"] as string;

            GameObject plate = Resources.Load("Prefabs/Obstacles/" + plateName) as GameObject;

            if (plate != null)
            {
                plate = Instantiate(plate) as GameObject;

                if (plate != null)
                {
                    ISerializable plateSerializer = plate.GetComponent(typeof(ISerializable)) as ISerializable;

                    plate.name = plateName;
                    plate.transform.parent = parent.transform;
                    plate.transform.localPosition = LevelEditorUtilities.StringToVector3(platePosition);
                    plate.transform.up = LevelEditorUtilities.StringToVector3(plateDirection);

                    if (plateSerializer != null)
                        plateSerializer.Unserialize(ref plateDict);

                    m_objectsLoaded++;

                    if (m_slowSimulation)
                        yield return new WaitForSeconds(m_simulationTime);
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructItems(Dictionary<string, object> chamberDict, Room room)
    {
        if (chamberDict != null && chamberDict.ContainsKey("items") && room != null)
        {
            Dictionary<string, object> itemsDict = chamberDict["items"] as Dictionary<string, object>;

            GameObject itemsContainer = new GameObject();

            if (itemsDict != null && itemsContainer != null)
            {
                room.SetChild(itemsContainer);

                itemsContainer.name = "Items";
                itemsContainer.transform.localPosition = Vector3.zero;
                
                yield return StartCoroutine(ConstructPowerCores(itemsDict, itemsContainer));
                yield return StartCoroutine(ConstructRedPotions(itemsDict, itemsContainer));
                yield return StartCoroutine(ConstructGreenPotions(itemsDict, itemsContainer));
                yield return StartCoroutine(ConstructCrates(itemsDict, itemsContainer));
            }
        }

        yield break;
    }

    private IEnumerator ConstructPowerCores(Dictionary<string, object> itemsDict, GameObject parent)
    {
        if (itemsDict != null && itemsDict.ContainsKey("powerCores") && parent != null)
        {
            List<object> powerCores = itemsDict["powerCores"] as List<object>;
            Dictionary<string, object> powerCoreDict = null;

            GameObject powerCoresContainer = new GameObject();

            if (powerCores != null && powerCoresContainer != null)
            {
                powerCoresContainer.name = "Cores";
                powerCoresContainer.transform.parent = parent.transform;
                powerCoresContainer.transform.localPosition = Vector3.zero;

                for (int i = 0; i < powerCores.Count; i++)
                {
                    powerCoreDict = powerCores[i] as Dictionary<string, object>;

                    if(powerCoreDict != null)
                        yield return StartCoroutine(ConstructPowerCore(powerCoreDict, powerCoresContainer));
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructPowerCore(Dictionary<string, object> powerCoreDict, GameObject parent)
    {
        if (powerCoreDict != null && parent != null)
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

    private IEnumerator ConstructRedPotions(Dictionary<string, object> itemsDict, GameObject parent)
    {
        if (itemsDict != null && itemsDict.ContainsKey("redPotions") && parent != null)
        {
            List<object> redPotions = itemsDict["redPotions"] as List<object>;
            Dictionary<string, object> redPotionDict = null;

            GameObject redPotionsContainer = new GameObject();

            if (redPotions != null && redPotionsContainer != null)
            {
                redPotionsContainer.name = "RedPotions";
                redPotionsContainer.transform.parent = parent.transform;
                redPotionsContainer.transform.localPosition = Vector3.zero;

                for (int i = 0; i < redPotions.Count; i++)
                {
                    redPotionDict = redPotions[i] as Dictionary<string, object>;

                    if(redPotionDict != null)
                        yield return StartCoroutine(ConstructRedPotion(redPotionDict, redPotionsContainer));
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructRedPotion(Dictionary<string, object> redPotionDict, GameObject parent)
    {
        if (redPotionDict != null && parent != null)
        {
            string redPotionName = redPotionDict["name"] as string;
            string redPotionPosition = redPotionDict["position"] as string;
            string redPotionDirection = redPotionDict["direction"] as string;

            GameObject redPotion = Resources.Load("Prefabs/Items/" + redPotionName) as GameObject;

            if (redPotion != null)
            {
                redPotion = Instantiate(redPotion) as GameObject;

                if (redPotion != null)
                {
                    redPotion.name = redPotionName;
                    redPotion.transform.parent = parent.transform;
                    redPotion.transform.localPosition = LevelEditorUtilities.StringToVector3(redPotionPosition);
                    redPotion.transform.up = LevelEditorUtilities.StringToVector3(redPotionDirection);

                    m_objectsLoaded++;

                    if (m_slowSimulation)
                        yield return new WaitForSeconds(m_simulationTime);
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructGreenPotions(Dictionary<string, object> itemsDict, GameObject parent)
    {
        if (itemsDict != null && itemsDict.ContainsKey("greenPotions") && parent != null)
        {
            List<object> greenPotions = itemsDict["greenPotions"] as List<object>;
            Dictionary<string, object> greenPotionDict = null;

            GameObject greenPotionsContainer = new GameObject();

            if (greenPotions != null && greenPotionsContainer != null)
            {
                greenPotionsContainer.name = "GreenPotions";
                greenPotionsContainer.transform.parent = parent.transform;
                greenPotionsContainer.transform.localPosition = Vector3.zero;

                for (int i = 0; i < greenPotions.Count; i++)
                {
                    greenPotionDict = greenPotions[i] as Dictionary<string, object>;

                    if(greenPotionDict != null)
                        yield return StartCoroutine(ConstructGreenPotion(greenPotionDict, greenPotionsContainer));
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructGreenPotion(Dictionary<string, object> greenPotionDict, GameObject parent)
    {
        if (greenPotionDict != null && parent != null)
        {
            string greenPotionName = greenPotionDict["name"] as string;
            string greenPotionPosition = greenPotionDict["position"] as string;
            string greenPotionDirection = greenPotionDict["direction"] as string;

            GameObject greenPotion = Resources.Load("Prefabs/Items/" + greenPotionName) as GameObject;

            if (greenPotion != null)
            {
                greenPotion = Instantiate(greenPotion) as GameObject;

                if (greenPotion != null)
                {
                    greenPotion.name = greenPotionName;
                    greenPotion.transform.parent = parent.transform;
                    greenPotion.transform.localPosition = LevelEditorUtilities.StringToVector3(greenPotionPosition);
                    greenPotion.transform.up = LevelEditorUtilities.StringToVector3(greenPotionDirection);

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
        if (itemsDict != null && itemsDict.ContainsKey("crates") && parent != null)
        {
            List<object> crates = itemsDict["crates"] as List<object>;
            Dictionary<string, object> crateDict = null;

            GameObject cratesContainer = new GameObject();

            if (crates != null && cratesContainer != null)
            {
                cratesContainer.name = "Crates";
                cratesContainer.transform.parent = parent.transform;
                cratesContainer.transform.localPosition = Vector3.zero;

                for (int i = 0; i < crates.Count; i++)
                {
                    crateDict = crates[i] as Dictionary<string, object>;

                    if(crateDict != null)
                        yield return StartCoroutine(ConstructCrate(crateDict, cratesContainer));
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructCrate(Dictionary<string, object> crateDict, GameObject parent)
    {
        if (crateDict != null && parent != null)
        {
            string crateName = crateDict["name"] as string;
            string cratePosition = crateDict["position"] as string;
            string crateDirection = crateDict["direction"] as string;

            GameObject crate = Resources.Load("Prefabs/Obstacles/" + crateName) as GameObject;

            if (crate != null)
            {
                crate = Instantiate(crate) as GameObject;

                if (crate != null)
                {
                    crate.name = crateName;
                    crate.transform.parent = parent.transform;
                    crate.transform.localPosition = LevelEditorUtilities.StringToVector3(cratePosition);
                    crate.transform.up = LevelEditorUtilities.StringToVector3(crateDirection);

                    m_objectsLoaded++;

                    if (m_slowSimulation)
                        yield return new WaitForSeconds(m_simulationTime);
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructWallColliders(Dictionary<string, object> chamberDict, Room room)
    {
        if (chamberDict != null && chamberDict.ContainsKey("wallColliders") && room != null)
        {
            List<object> wallColliders = chamberDict["wallColliders"] as List<object>;
            Dictionary<string, object> wallColliderDict = null;

            GameObject wallCollidersContainer = new GameObject();

            if (wallColliders != null && wallCollidersContainer != null)
            {
                m_dynamicWalls = new List<GameObject>();

                room.SetChild(wallCollidersContainer);

                wallCollidersContainer.name = "WallColliders";
                wallCollidersContainer.transform.localPosition = Vector3.zero;
                wallCollidersContainer.tag = "Wall";

                for (int i = 0; i < wallColliders.Count; i++)
                {
                    wallColliderDict = wallColliders[i] as Dictionary<string, object>;

                    if (wallColliderDict != null)
                        yield return StartCoroutine(ConstructWallCollider(wallColliderDict, wallCollidersContainer));
                }
            }
        }

        yield break;
    }

    private IEnumerator ConstructWallCollider(Dictionary<string, object> wallColliderDict, GameObject parent)
    {
        if (wallColliderDict != null && parent != null)
        {
            string wallColliderPosition = wallColliderDict["position"] as string;
            string wallColliderSize = wallColliderDict["size"] as string;

            BoxCollider2D wallCollider = parent.AddComponent<BoxCollider2D>();

            if (wallCollider != null)
            {
                wallCollider.center = LevelEditorUtilities.StringToVector2(wallColliderPosition);
                wallCollider.size = LevelEditorUtilities.StringToVector2(wallColliderSize);

                m_objectsLoaded++;

                if (m_slowSimulation)
                    yield return new WaitForSeconds(m_simulationTime);
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

        // Temporary until the efficiency checks are put into place.
        if(m_doors != null)
            m_doors.Clear();

        if(m_dynamicWalls != null)
            m_dynamicWalls.Clear();
    }

    private List<GameObject> _GetDoors()
    {
        return m_doors;
    }

    private List<GameObject> _GetDynamicWalls()
    {
        return m_dynamicWalls;
    }

    public void OnGUI()
    {
        if (m_isLoading)
        {
            if (m_objectsToLoad > 0)
                m_loadingPercentage = Mathf.Floor(((float)m_objectsLoaded / m_objectsToLoad) * 100f);

            m_loadingText = m_loadingMessage.Replace("%p", m_loadingPercentage.ToString() + "%");

            GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height - 50f), m_loadingText, m_style);
        }
    }

    #region Interface Methods
    public static void Load(string levelName)
    {
        // This is only to make the GameManager happy.
    }

    public static void LoadNextLevel()
    {
        SceneManager.Get()._LoadNextLevel();
    }

    public static List<GameObject> GetDoors()
    {
        return SceneManager.Get()._GetDoors();
    }

    public static List<GameObject> GetDynamicWalls()
    {
        return SceneManager.Get()._GetDynamicWalls();
    }
    #endregion
}
