using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ResponseType
{
    Okay,
    YesNo
}

public enum Response
{
    Okay,
    Yes,
    No,
}

public class ModalMenu : MonoBehaviour
{
    private struct ModalData
    {
        public string title;
        public string description;

        public Action<Response> onComplete;

        public ResponseType responseType;

        public Sprite headerImage;
        public Sprite bodyImage;
    };

    public static ModalMenu Instance { get; private set; }

    private static Queue<ModalData> menuQueue = new Queue<ModalData>();

    private ModalData currentMenuData;

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject background;

    [SerializeField] private Image imgHeader;
    [SerializeField] private Image imgBody;

    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TextMeshProUGUI txtbody;

    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnOkay;
    [SerializeField] private Button btnYes;
    [SerializeField] private Button btnNo;

    //
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        btnClose.onClick.AddListener(
            delegate
            {
                OnUserInput(currentMenuData.responseType == ResponseType.Okay ? Response.Okay : Response.No);
            });

        btnOkay.onClick.AddListener(
            delegate
            {
                OnUserInput(Response.Okay);
            });

        btnYes.onClick.AddListener(
            delegate
            {
                OnUserInput(Response.Yes);
            });

        btnNo.onClick.AddListener(
            delegate
            {
                OnUserInput(Response.No);
            }); 
    }

    private void Update()
    {
        if(menuQueue.Count > 0 && !panel.activeInHierarchy)
        {
            //Time to take the first modal out and show it.
            currentMenuData = menuQueue.Dequeue();

            background.SetActive(true);

            UpdateUIContent();

            OnShow();
        }
    }

    protected virtual void OnShow()
    {
        panel.SetActive(true);
    }

    protected virtual void OnHide()
    {
        panel.SetActive(false);

        if(menuQueue.Count <= 0)
        {
            background.SetActive(false);
        }
    }

    private void UpdateUIContent()
    {
        if(currentMenuData.headerImage == null)
        {
            imgHeader.gameObject.SetActive(false);
        }
        else
        {
            imgHeader.sprite = currentMenuData.headerImage;
            imgHeader.gameObject.SetActive(true);
        }

        if(currentMenuData.bodyImage == null)
        {
            imgBody.gameObject.SetActive(false);
        }
        else
        {
            imgBody.sprite = currentMenuData.bodyImage;
            imgBody.gameObject.SetActive(true);
        }

        if(string.IsNullOrEmpty(currentMenuData.title))
        {
            txtTitle.gameObject.SetActive(false);
        }
        else
        {
            txtTitle.text = currentMenuData.title;
            txtTitle.gameObject.SetActive(true);
        }

        if(string.IsNullOrEmpty(currentMenuData.description))
        {
            txtbody.gameObject.SetActive(false);
        }
        else
        {
            txtbody.text = currentMenuData.description;
            txtbody.gameObject.SetActive(true);
        }

        switch (currentMenuData.responseType)
        {
            default:
            case ResponseType.Okay:
                btnOkay.gameObject.SetActive(true);

                btnNo.gameObject.SetActive(false);
                btnYes.gameObject.SetActive(false);
                break;

            case ResponseType.YesNo:
                btnOkay.gameObject.SetActive(false);

                btnNo.gameObject.SetActive(true);
                btnYes.gameObject.SetActive(true);
                break;
        }
    }

    private void OnUserInput(Response _response)
    {
        currentMenuData.onComplete?.Invoke(_response);

        OnHide();
    }

    public static void Show(string _title, string _description)
    {
        Show(_title, _description, ResponseType.Okay);
    }

    public static void Show(string _title, string _description, ResponseType _type, Action<Response> _onUserInput = null)
    {
        Show(_title, _description, _type, _onUserInput, null, null);
    }

    public static void Show(string _title, string _description, ResponseType _type, Sprite _contentImage, bool _isHeaderImage, Action<Response> _onUserInput = null)
    {
        Show(_title, _description, _type, _onUserInput, _isHeaderImage ? _contentImage : null, _isHeaderImage ? null : _contentImage);
    }

    public static void Show(string _title, string _description, ResponseType _type, Action<Response> _onUserInput, Sprite _headerImage, Sprite _bodyImage)
    {
        ModalData _newModal = new ModalData()
        {
            title = _title,
            description = _description,
            responseType = _type,
            onComplete = _onUserInput,
            headerImage = _headerImage,
            bodyImage = _bodyImage
        };

        menuQueue.Enqueue(_newModal);
    }
}
