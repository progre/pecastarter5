
namespace CreteLib
{
    /// <summary>
    /// ノードの種類
    /// Tag:開始タグ、終了タグがセットになったタグノード
    /// EndTag:記述ミスなどにより、終了タグのみがのこったもの
    /// Text：タグにはさまれたテキスト
    /// SelfComplete自己完結しているタグ
    /// </summary>
    public enum NodeType
    {
        Tag,EndTag,Text,SelfComplete
    }
}
