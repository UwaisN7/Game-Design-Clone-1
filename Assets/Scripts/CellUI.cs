using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI codeText;
    [SerializeField] private Image bgImage;
    [SerializeField] private Button button;

    public Vector2Int GridPos { get; private set; }
    public int Code { get; private set; }
    public bool IsUsed { get; private set; }

    private System.Action<CellUI> onClickCallback;

    public void Setup(int code, Vector2Int pos, System.Action<CellUI> callback)
    {
        Code = code;
        GridPos = pos;
        IsUsed = false;
        codeText.text = code.ToString("D2"); // Renders purely as 10, 20, 30, etc.
        onClickCallback = callback;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickCallback?.Invoke(this));
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
        button.interactable = false;
        codeText.text = "00"; // Replaces the number with 00 when clicked
        bgImage.color = new Color(0.15f, 0.15f, 0.15f, 0.5f);
    }

    public void SetHighlight(bool isSelectable)
    {
        if (IsUsed) return;

        button.interactable = isSelectable;

        if (isSelectable)
        {
            bgImage.color = new Color(0f, 0.85f, 0.8f, 1f); // Bright Cyan for active row/col
            codeText.color = Color.black;
        }
        else
        {
            bgImage.color = new Color(0.08f, 0.12f, 0.16f, 1f); // Dark for locked cells
            codeText.color = new Color(0.3f, 0.5f, 0.5f, 1f);
        }
    }
}