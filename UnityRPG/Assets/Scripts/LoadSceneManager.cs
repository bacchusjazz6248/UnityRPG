
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{

    //　フェードスピード
    public float fadeSpeed = 2.0f;
    //　アンロードするシーン
    private Scene unLoadScene;
    //　フェードイメージ
    public Image fadeImage;
    //　戦闘終了ボタン
    public GameObject button;
    //　シーン用データ
    private SceneData sceneData;

    IEnumerator Start()
    {
        //　最初にWorldシーンを読み込む
        yield return LoadNewScene("AdventureScene");
        //　SceneDataを保持
        sceneData = FindObjectOfType(typeof(SceneData)) as SceneData;
    }

    public void FadeAndLoadScene(string sceneName)
    {
        //　個々のシーンのデータを取得
        StartCoroutine(LoadScene(sceneName));
    }

    IEnumerator LoadScene(string sceneName)
    {

        //　戦闘終了ボタンが設定されていれば無効
        if (sceneData.button != null)
        {
            sceneData.button.SetActive(false);
        }
        //　現在のシーンデータを取得
        sceneData = FindObjectOfType(typeof(SceneData)) as SceneData;
        //　他のシーンへ遷移する時にフェードアウト
        yield return StartCoroutine(Fade(sceneData.fadeImage, 1f));

        Destroy(FindObjectOfType(typeof(AudioListener)));
        unLoadScene = SceneManager.GetActiveScene();
        //　フェードアウトが完了したら新しいシーンを読み込む
        yield return StartCoroutine(LoadNewScene(sceneName));
        //　フェードアウトが完了したら前のシーンをアンロード
        yield return StartCoroutine(UnLoadScene());

        //　現在のシーンデータを取得
        sceneData = FindObjectOfType(typeof(SceneData)) as SceneData;

        //　Battleシーンの時だけフェードイン
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("BattleScene"))
        {
            yield return StartCoroutine(Fade(sceneData.fadeImage, 0f));
        }

        if (sceneData.button != null)
        {
            //　フェードイン後に戦闘終了ボタンを有効にする
            if (SceneManager.GetActiveScene().buildIndex == SceneManager.GetSceneByName("BattleScene").buildIndex)
            {
                sceneData.button.SetActive(true);
            }
            else
            {
                sceneData.button.SetActive(false);
            }
        }
    }

    public IEnumerator Fade(Image fadeImage, float alpha)
    {

        //　目的のアルファ値になるまで徐々に変化させる
        while (!Mathf.Approximately(fadeImage.color.a, alpha))
        {
            fadeImage.color = new Color(0f, 0f, 0f, Mathf.MoveTowards(fadeImage.color.a, alpha, fadeSpeed * Time.deltaTime));
            yield return null;
        }
    }

    //　新しいシーンをロード
    IEnumerator LoadNewScene(string sceneName)
    {

        //　シーン読み込み処理
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!async.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("AdventureScene"))
        {
            (FindObjectOfType(typeof(GenerateEnemy)) as GenerateEnemy).InstantiateEnemy();
        }
    }

    //　シーンのアンロード
    IEnumerator UnLoadScene()
    {
        yield return SceneManager.UnloadScene(unLoadScene.buildIndex);
    }
}
