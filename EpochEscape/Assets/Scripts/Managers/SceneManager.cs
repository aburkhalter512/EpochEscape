using UnityEngine;
using System.Collections;

public class SceneManager : Manager<SceneManager>
{
    protected override void Initialize()
    {
        // First time initialization.
    }

    private void _StartLoading(string name)
    {
        StartCoroutine(_Load(name));
    }

    private IEnumerator _Load(string name)
    {
        FadeManager.StartAlphaFade(Color.black, false, 2f, 0f, () => { Application.LoadLevel("Loading"); });

        yield return new WaitForSeconds(2.5f);

        FadeManager.StartAlphaFade(Color.black, false, 2f, 0f, () => { Application.LoadLevel(name); });
    }

    public static void Load(string name)
    {
        SceneManager.GetInstance()._StartLoading(name);
    }
}
