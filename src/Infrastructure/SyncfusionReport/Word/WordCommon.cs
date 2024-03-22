using TD.WebApi.Shared.Common;

namespace TD.WebApi.Infrastructure.SyncfusionReport.Word;
public class ITemplateWord
{
    public int IndexTable { set; get; }
    public int IndexRow { set; get; }
    public List<ITempTagWord> ITempTag { get; set; }

    public ITemplateWord()
    {
    }

    public ITemplateWord(int indexTable, int indexRow, List<ITempTagWord> iTempTag)
    {
        IndexTable = indexTable;
        IndexRow = indexRow;
        ITempTag = iTempTag;
    }
}

public class ITempTagWord
{
    public string TagName { get; set; }
    public string TagType { get; set; }
    public string TagStyle { get; set; }

    public ITempTagWord(string tagName, string tagType, string tagStyle)
    {
        TagName = tagName;
        TagType = tagType;
        TagStyle = tagStyle;
    }
}

public class ITableWord
{
    public int IndexTable { get; set; }
    public int CountRow { get; set; }

    public ITableWord(int indexTable, int countRow)
    {
        IndexTable = indexTable;
        CountRow = countRow;
    }
}

public class IRepeatWord
{
    public ITemplateWord ITemp { get; set; }
    public object Data { get; set; }

    public IRepeatWord(ITemplateWord iTemp, object data)
    {
        ITemp = iTemp;
        Data = data;
    }
}

public class ITagWord
{
    public string TagName { get; set; }
    public object Data { get; set; }
    public TagWordType TagType { get; set; }
    public string TagStyle { get; set; }

    public ITagWord()
    {
    }

    public ITagWord(string tagName, object data, TagWordType tagType, string tagStyle)
    {
        TagName = tagName;
        Data = data;
        TagType = tagType;
        TagStyle = tagStyle;
    }
}

public class ListCellOfGroup
{
    public int IndexTable { get; set; }
    public int IndexCell { get; set; }

    public ListCellOfGroup(int indexTable, int indexCell)
    {
        IndexTable = indexTable;
        IndexCell = indexCell;
    }
}

public class CountItemInGroup
{
    public int IndexTable { get; set; }
    public int IndexCell { get; set; }
    public int[] LstRows { get; set; }

    public CountItemInGroup(int indexTable, int indexCell, int[] lstRows)
    {
        IndexTable = indexTable;
        IndexCell = indexCell;
        LstRows = lstRows;
    }
}
#region Enum




#endregion