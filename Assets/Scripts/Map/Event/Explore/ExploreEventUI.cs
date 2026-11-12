using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExploreEventUI : MonoBehaviour
{
    [System.Serializable]
    public class ExploreObject
    {
        public Button button;
        public GameObject openNote;
        public Image tokenImage;
        public TMP_Text tokenDescription;

        [HideInInspector] public TokenDataSO assignedToken;
        [HideInInspector] public bool isOpened;
    }

    [Header("탐색 오브젝트 4개")]
    [SerializeField] private ExploreObject[] objects = new ExploreObject[4];

    [Header("나가기 버튼")]
    [SerializeField] private Button clearButton;

    private int openedCount;
    private const int MaxSelections = 2;

    private void OnEnable()
    {
        InitializeExplore();
    }

    private void InitializeExplore()
    {
        openedCount = 0;

        if (clearButton != null)
            clearButton.interactable = false;

        if (EventTokenManager.Instance == null)
        {
            Debug.LogError("EventTokenManager가 없습니다.");
            return;
        }

        List<TokenDataSO> pool =
            EventTokenManager.Instance.GetTokensBySource(TokenSource.Explore);

        pool.RemoveAll(token => token == null);
        pool = new List<TokenDataSO>(new HashSet<TokenDataSO>(pool));

        if (objects.Length != 4 || pool.Count < 4)
        {
            Debug.LogError("탐색 오브젝트 4개와 서로 다른 탐색 증표 4개 이상이 필요합니다.");
            return;
        }

        for (int i = pool.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (pool[i], pool[randomIndex]) = (pool[randomIndex], pool[i]);
        }

        for (int i = 0; i < objects.Length; i++)
        {
            ExploreObject obj = objects[i];

            obj.assignedToken = pool[i];
            obj.isOpened = false;

            obj.openNote.SetActive(false);
            obj.button.interactable = true;

            int index = i;

            obj.button.onClick.RemoveAllListeners();
            obj.button.onClick.AddListener(() => OpenObject(index));
        }
    }

    private void OpenObject(int index)
    {
        ExploreObject obj = objects[index];

        if (obj.isOpened || openedCount >= MaxSelections)
            return;

        if (!EventTokenManager.Instance.AddToken(obj.assignedToken))
        {
            Debug.LogWarning("증표를 획득하지 못했습니다.");
            return;
        }

        obj.isOpened = true;
        openedCount++;

        obj.tokenImage.sprite = obj.assignedToken.tokenIcon;
        obj.tokenDescription.text = obj.assignedToken.description;

        obj.openNote.SetActive(true);
        obj.button.interactable = false;

        if (openedCount >= MaxSelections)
        {
            foreach (ExploreObject other in objects)
                other.button.interactable = false;

            clearButton.interactable = true;
        }
    }
}