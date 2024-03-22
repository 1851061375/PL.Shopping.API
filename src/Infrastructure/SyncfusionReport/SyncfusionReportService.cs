using Microsoft.Extensions.Logging;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using System.Collections;
using System.Reflection;
using TD.WebApi.Application.Common.Exporters;
using TD.WebApi.Infrastructure.SyncfusionReport.Common;
using TD.WebApi.Infrastructure.SyncfusionReport.Word;
using TD.WebApi.Shared.Common;

namespace TD.WebApi.Infrastructure.SyncfusionReport;
public class SyncfusionReportService : ISyncfusionReportService
{
    private readonly ILogger<SyncfusionReportService> _logger;

    public SyncfusionReportService(ILogger<SyncfusionReportService> logger)
    {
        _logger = logger;
    }

    public Stream GetWordFileReport(string? templateInputPath, byte[]? templateInputFile, object data, WordType type = WordType.Docx)
    {
        var memory = new MemoryStream();
        WordDocument document;
        WSection section;

        List<ITemplateWord> lstAllTemplate = new List<ITemplateWord>();
        List<ITemplateWord> lstTemplate = new List<ITemplateWord>();
        List<IRepeatWord> lstRepeat = new List<IRepeatWord>();
        List<ITagWord> lstTag = new List<ITagWord>();
        List<CountItemInGroup> lstItemInGroup = new List<CountItemInGroup>();
        List<ListCellOfGroup> lstGroup = new List<ListCellOfGroup>();
        List<CountItemInGroup> lstItemLast = new List<CountItemInGroup>();
        List<ITableWord> lstTable = new List<ITableWord>();

        using (var stream = File.OpenRead(templateInputPath!))
        {
            document = new WordDocument(stream, FormatType.Docx);
            section = document.LastSection;
        }

        for (int i = 0, n = section.Tables.Count; i < n; i++)
        {
            int indexRow = 0;
            lstTable.Add(new ITableWord(i, section.Tables[i].Rows.Count));
            for (int j = 0, m = section.Tables[i].Rows.Count; j < m; j++)
            {
                indexRow = j;
                List<ITempTagWord> tmpcell = new List<ITempTagWord>();
                for (int k = 0, l = section.Tables[i].Rows[j].Cells.Count; k < l; k++)
                {
                    GetArrTemp(i, k, section.Tables[i].Rows[j].Cells[k], tmpcell, lstGroup);
                }

                if (tmpcell.Any())
                {
                    lstAllTemplate.Add(new ITemplateWord(i, indexRow, tmpcell));
                }
            }
        }

        PropertyInfo[] properties = data.GetType().GetProperties();

        foreach (PropertyInfo property in properties)
        {
            Type propertyType = property.PropertyType;
            string propertyName = property.Name;

            if ((propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>)) || (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IList<>)))
            {
                object? propertyValue = property.GetValue(data, null);

                if (propertyValue != null)
                {
                    Type tmp = propertyValue.GetType();
                }

                if (propertyValue != null && propertyValue is IEnumerable collection)
                {
                    foreach (object item in collection)
                    {
                        IRepeatWord? repeat = GetTableOfIT(item, lstAllTemplate, lstTemplate);
                        if (repeat != null)
                        {
                            lstRepeat.Add(repeat);
                        }
                    }
                }
            }
            else
            {
                object? propertyValue = property.GetValue(data, null);
                if (!string.IsNullOrEmpty(propertyName) && propertyValue != null)
                {
                    GetPropertyOfTag(propertyName.Trim(), propertyValue, document, lstAllTemplate, lstTag);
                }

            }
        }

        #region BindDataRepeat
        if (lstRepeat.Count > 0)
        {
            int indexInsert, indexTable;
            string paragraph = string.Empty;
            foreach (IRepeatWord iRepeat in lstRepeat)
            {
                indexTable = iRepeat.ITemp.IndexTable;
                indexInsert = section.Tables[indexTable].Rows.Count;
                WTableRow r = section.Tables[indexTable].Rows[iRepeat.ITemp.IndexRow].Clone();
                section.Tables[indexTable].Rows.Insert(indexInsert, r);
                WTableCell cell;
                for (int i = 0, n = section.Tables[indexTable].Rows[indexInsert].Cells.Count; i < n; i++)
                {
                    cell = section.Tables[indexTable].Rows[indexInsert].Cells[i];
                    ReplateTempInCell(cell, iRepeat, document);
                }
            }
        }
        #endregion

        #region SetTagValue

        byte[] imgData;

        foreach (ITagWord iTag in lstTag)
        {
            try
            {
                string tagName = Contains.START_TAG + iTag.TagName + Contains.END_TAG;
                switch (iTag.TagType)
                {
                    case TagWordType.Text:
                        document.Replace(tagName, iTag.Data + string.Empty, false, false);
                        break;
                    case TagWordType.Image:
                        byte[] img = Convert.FromBase64String(iTag.Data + string.Empty);
                        IWParagraph paragraph = section.AddParagraph();
                        IWPicture picture = paragraph.AppendPicture(img);
                        if (iTag.TagStyle != string.Empty)
                        {
                            if (iTag.TagStyle.IndexOf(';') != -1)
                            {
                                string[] lstStyle = iTag.TagStyle.Split(
                                    new char[] { ';' },
                                    StringSplitOptions.RemoveEmptyEntries);

                                for (int i = 0, n = lstStyle.Length; i < n; i++)
                                {
                                    SetStyle(picture, lstStyle[i]);
                                }
                            }
                            else
                            {
                                SetStyle(picture, iTag.TagStyle);
                            }
                        }

                        paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        TextBodyPart textBodyPart = new TextBodyPart(document);
                        textBodyPart.BodyItems.Add(paragraph);
                        document.Replace(tagName, textBodyPart, false, false);
                        break;
                    /*case TagWordType.BarCode:
                        Barcode b = new Barcode(iTag.Data + "", TYPE.CODE128)
                        {
                            IncludeLabel = true,
                            ForeColor = Color.Black,
                            BackColor = Color.White,
                        };
                        imgData = b.GetImageData(SaveTypes.JPG);

                        IWParagraph paraBarcode = Section.AddParagraph();
                        IWPicture pictureBarcode = paraBarcode.AppendPicture(imgData);
                        if (iTag.TagStyle != "")
                        {
                            if (iTag.TagStyle.IndexOf(';') != -1)
                            {
                                string[] lstStyle = iTag.TagStyle.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 0, n = lstStyle.Length; i < n; i++)
                                    SetStyle(pictureBarcode, lstStyle[i]);
                            }
                            else SetStyle(pictureBarcode, iTag.TagStyle);
                        }
                        paraBarcode.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        TextBodyPart textBodyPartBarcode = new TextBodyPart(Document);
                        textBodyPartBarcode.BodyItems.Add(paraBarcode);
                        Document.Replace(tagName, textBodyPartBarcode, false, false);
                        break;
                    case TagWordType.QRCode:
                        QRCodeEncoder qrCodeEncoder = new QRCodeEncoder
                        {
                            QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC,
                            QRCodeScale = 3,//Size
                            QRCodeVersion = 3,//Version
                            QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M,
                        };

                        Image qrcodeImage = qrCodeEncoder.Encode(iTag.Data + "");
                        imgData = GetImageData(qrcodeImage);

                        IWParagraph paraQRCode = Section.AddParagraph();
                        IWPicture pictureQRCode = paraQRCode.AppendPicture(imgData);
                        if (iTag.TagStyle != "")
                        {
                            if (iTag.TagStyle.IndexOf(';') != -1)
                            {
                                string[] lstStyle = iTag.TagStyle.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 0, n = lstStyle.Length; i < n; i++)
                                    SetStyle(pictureQRCode, lstStyle[i]);
                            }
                            else SetStyle(pictureQRCode, iTag.TagStyle);
                        }
                        paraQRCode.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        TextBodyPart textBodyQRCode = new TextBodyPart(Document);
                        textBodyQRCode.BodyItems.Add(paraQRCode);
                        Document.Replace(tagName, textBodyQRCode, false, false);
                        break;*/
                    default:
                        break;
                }
            }
            catch
            {
            }
        }

        #endregion

        #region DeleteItemplates
        for (int i = 0, n = lstTable.Count(); i < n; i++)
        {
            var rowTemp = lstRepeat.Where(x => x.ITemp.IndexTable == i).GroupBy(x => x.ITemp.IndexRow).Select(x => x.First()).OrderByDescending(x => x.ITemp.IndexRow);
            foreach (IRepeatWord repeat in rowTemp)
                section.Tables[i].Rows.RemoveAt(repeat.ITemp.IndexRow);
        }
        #endregion

        #region DeleteTags
        string text = string.Empty;
        for (int a = 0, b = document.Sections.Count; a < b; a++)
        {
            for (int i = 0, n = document.Sections[a].Paragraphs.Count; i < n; i++)
            {
                text = document.Sections[a].Paragraphs[i].Text;
                if (!string.IsNullOrEmpty(text) && text.IndexOf(Contains.START_TAG) != -1 && text.IndexOf(Contains.END_TAG) != -1 && text.IndexOf(Contains.START_TAG) < text.IndexOf(Contains.END_TAG))
                {
                    while (text.IndexOf(Contains.START_TAG) != -1 && text.IndexOf(Contains.END_TAG) != -1 && text.IndexOf(Contains.START_TAG) < text.IndexOf(Contains.END_TAG))
                    {
                        text = text.Remove(text.IndexOf(Contains.START_TAG), 1 + text.IndexOf(Contains.END_TAG) - text.IndexOf(Contains.START_TAG));
                    }

                    document.Sections[a].Paragraphs[i].Text = text;
                }
            }

            for (int i = 0, n = document.Sections[a].Tables.Count; i < n; i++)
            {
                foreach (WTableRow row in document.Sections[a].Tables[i].Rows)
                {
                    foreach (WTableCell cell in row.Cells)
                    {
                        foreach (IWParagraph para in cell.Paragraphs)
                        {
                            text = para.Text;
                            if (!string.IsNullOrEmpty(text) && text.IndexOf(Contains.START_TAG) != -1 && text.IndexOf(Contains.END_TAG) != -1 && text.IndexOf(Contains.START_TAG) < text.IndexOf(Contains.END_TAG))
                            {
                                while (text.IndexOf(Contains.START_TAG) != -1 && text.IndexOf(Contains.END_TAG) != -1 && text.IndexOf(Contains.START_TAG) < text.IndexOf(Contains.END_TAG))
                                {
                                    text = text.Remove(text.IndexOf(Contains.START_TAG), 1 + text.IndexOf(Contains.END_TAG) - text.IndexOf(Contains.START_TAG));
                                }

                                para.Text = text;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region MergeGroupInTable

        var tableIndexs = lstGroup.GroupBy(x => x.IndexTable).Select(y => y.First()).Select(z => z.IndexTable);
        int startIndex, endIndex;
        foreach (int tableIndex in tableIndexs)
        {
            IWTable table = section.Tables[tableIndex];
            startIndex = 1;
            endIndex = table.Rows.Count;
            List<ListCellOfGroup> subGroup = lstGroup.Where(x => x.IndexTable == tableIndex).OrderBy(y => y.IndexCell).ToList();

            if (endIndex > 2)
            {
                MergeCell(table, subGroup, startIndex, endIndex);
            }
        }
        #endregion

        FormatType fileType = type == WordType.Docx ? FormatType.Docx : FormatType.Doc;

        document.Save(memory, fileType);

        document.Close();
        return memory;
    }

    private void ReplateTempInCell(WTableCell cell, IRepeatWord iRepeat, WordDocument document)
    {
        PropertyInfo[] pptI;
        List<string> str = new List<string>();
        TextBodyPart textBodyPart = new TextBodyPart(document);
        pptI = iRepeat.Data.GetType().GetProperties();
        int length = pptI.Count();
        for (int i = 0, n = cell.Paragraphs.Count; i < n; i++)
        {
            string paragraph = cell.Paragraphs[i].Text;
            for (int j = 0; j < length; j++)
            {
                str.Add("{" + pptI[j].Name + "}");
            }

            string[] lst = str.ToArray();
            for (int j = 0; j < lst.Length; j++)
            {
                if (paragraph.IndexOf(lst[j]) != -1)
                {
                    string tagType = "Text";
                    string tagStyle = string.Empty;
                    for (int k = 0, l = iRepeat.ITemp.ITempTag.Count; k < l; k++)
                    {
                        if (pptI[j].Name == iRepeat.ITemp.ITempTag[k].TagName)
                        {
                            tagType = iRepeat.ITemp.ITempTag[k].TagType;
                            tagStyle = iRepeat.ITemp.ITempTag[k].TagStyle;
                        }
                    }

                    if (tagType.ToLower() == TagWordType.Text.ToString().ToLower())
                    {
                        paragraph = paragraph.Replace(lst[j], pptI[j].GetValue(iRepeat.Data, null) + string.Empty);
                        cell.Paragraphs[i].Text = paragraph;
                    }
                    else if (tagType.ToLower() == TagWordType.Group.ToString().ToLower())
                    {
                        paragraph = paragraph.Replace(lst[j], pptI[j].GetValue(iRepeat.Data, null) + string.Empty);
                        cell.Paragraphs[i].Text = paragraph;
                    }
                    else if (tagStyle.ToLower() == TagWordType.Image.ToString().ToLower())
                    {
                        string strImg = pptI[j].GetValue(iRepeat.Data, null) + string.Empty;
                        string s = cell.Paragraphs[i].Text;
                        string[] lsts = s.Split(new string[] { lst[j] }, StringSplitOptions.None);
                        cell.Paragraphs[i].Text = string.Empty;
                        cell.Paragraphs[i].AppendText(lsts[0]);
                        if (string.IsNullOrEmpty(strImg))
                        {
                            cell.Paragraphs[i].AppendText(string.Empty);
                            if (lsts.Length != 1)
                                cell.Paragraphs[i].AppendText(lsts[1]);
                        }
                        else
                        {
                            // Image img = ConvertStrBase64ToImage(strImg);
                            byte[] imgData = Convert.FromBase64String(strImg);
                            cell.Paragraphs[i].Text = string.Empty;
                            cell.Paragraphs[i].AppendText(lsts[0]);
                            IWPicture picture = cell.Paragraphs[i].AppendPicture(imgData);
                            if (lsts.Length != 1)
                                cell.Paragraphs[i].AppendText(lsts[1]);
                            if (!string.IsNullOrEmpty(tagStyle))
                            {
                                if (tagStyle.IndexOf(';') != -1)
                                {
                                    string[] lstStyle = tagStyle.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                    for (int k = 0, l = lstStyle.Length; k < l; k++)
                                        SetStyle(picture, lstStyle[k]);
                                }
                                else
                                {
                                    SetStyle(picture, tagStyle);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void SetStyle(IWPicture picture, string style)
    {
        string[] lstStyle = style.Split('-');
        switch (lstStyle[0].ToLower())
        {
            case "w":
                picture.Width = float.Parse(lstStyle[1]);
                break;
            case "h":
                picture.Height = float.Parse(lstStyle[1]);
                break;
            default:
                break;
        }
    }

    private void GetArrTemp(int indexTable, int indexCell, WTableCell cell, List<ITempTagWord> tmpcell, List<ListCellOfGroup> lstGroup)
    {
        for (int i = 0, n = cell.Paragraphs.Count; i < n; i++)
        {
            string paragraph = cell.Paragraphs[i].Text;
            if (paragraph.IndexOfAny(new char[] { '{', '}' }) != -1)
            {
                string[] lstTemp = paragraph.Split('{');
                for (int j = 0, m = lstTemp.Length; j < m; j++)
                {
                    if (lstTemp[j].IndexOf('}') != -1)
                    {
                        bool flag = false;
                        string str = lstTemp[j].Split('}')[0];
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str.IndexOf(":") == -1)
                            {
                                foreach (ITempTagWord itt in tmpcell)
                                {
                                    if (itt.TagName == str)
                                    {
                                        flag = true;
                                    }
                                }
                                if (!flag)
                                    tmpcell.Add(new ITempTagWord(str, TagWordType.Text.ToString(), ""));
                            }
                            else
                            {
                                string[] arrProperties = str.Split(':');
                                string tagStyle = "";
                                if (arrProperties.Length > 2)
                                    tagStyle = arrProperties[2] == null ? "" : arrProperties[2];
                                string tagName = arrProperties[1];
                                string tagType = arrProperties[0];
                                foreach (ITempTagWord itt in tmpcell)
                                {
                                    if (itt.TagName == tagName)
                                        flag = true;
                                }
                                if (!flag)
                                    tmpcell.Add(new ITempTagWord(tagName, tagType, tagStyle));
                                if (tagType == TagWordType.Group.ToString())
                                    lstGroup.Add(new ListCellOfGroup(indexTable, indexCell));
                                paragraph = paragraph.Replace(str, tagName);
                            }
                        }
                    }
                }
                cell.Paragraphs[i].Text = paragraph;
            }
        }
    }

    private IRepeatWord? GetTableOfIT(object obj, List<ITemplateWord> lstAllTemplate, List<ITemplateWord> lstTemplate)
    {
        PropertyInfo[] pptI;
        List<string> str = new List<string>();
        pptI = obj.GetType().GetProperties();
        int length = pptI.Count();
        for (int i = 0; i < length; i++)
        {
            str.Add(pptI[i].Name);
        }

        foreach (ITemplateWord itemp in lstAllTemplate)
        {
            List<string> temp = new List<string>();
            for (int j = 0, m = itemp.ITempTag.Count; j < m; j++)
            {
                temp.Add(itemp.ITempTag[j].TagName);
            }

            bool flag = CompareArrays(str.ToArray(), temp.ToArray());
            if (flag)
            {
                lstTemplate.Add(itemp);
                return new IRepeatWord(itemp, obj);
            }
        }

        return null;
    }

    private bool CompareArrays(string[] b, string[] a)
    {
        if (a.Length > b.Length) { return false; }
        for (int i = 0, n = a.Length; i < n; i++)
        {
            if (!b.Contains(a[i]))
                return false;
        }

        return true;
    }

    private void GetPropertyOfTag(string tagName, object obj, WordDocument document, List<ITemplateWord> lstAllTemplate, List<ITagWord> lstTag)
    {
        string temp = Contains.START_TAG + TagWordType.Image.ToString() + Contains.CHARACTER + tagName;
        if (document.Find(Contains.START_TAG + tagName + Contains.END_TAG, false, false) != null)
        {
            TagWordType type = TagWordType.Text;
            string style = string.Empty;

            List<ITemplateWord> template = lstAllTemplate
                .Select(m => new ITemplateWord
                {
                    ITempTag = m.ITempTag.Where(u => u.TagName == tagName).ToList()
                }).Where(y => y.ITempTag.Count > 0).ToList();
            if (template.Count > 0)
            {
                ITemplateWord? tempWord = template.ToList().FirstOrDefault() as ITemplateWord;
                string tagType = tempWord.ITempTag.FirstOrDefault().TagType.ToLower();
                if (tagType.Equals("image"))
                    type = TagWordType.Image;
                else if (tagType.Equals("barcode"))
                    type = TagWordType.BarCode;
                else if (tagType.Equals("qrcode"))
                    type = TagWordType.QRCode;
                else
                    type = TagWordType.Text;
                style = tempWord.ITempTag.FirstOrDefault().TagStyle;
            }

            lstTag.Add(new ITagWord(tagName, obj, type, style));
        }
        else if (document.Find(Contains.START_TAG + TagWordType.Image.ToString() + tagName + Contains.END_TAG, false, false) != null)
        {
            lstTag.Add(new ITagWord(tagName, obj, TagWordType.Image, string.Empty));
        }
        else if (document.Find(Contains.START_TAG + TagWordType.BarCode.ToString() + tagName + Contains.END_TAG, false, false) != null)
        {
            lstTag.Add(new ITagWord(tagName, obj, TagWordType.BarCode, string.Empty));
        }
        else if (document.Find(Contains.START_TAG + TagWordType.QRCode.ToString() + tagName + Contains.END_TAG, false, false) != null)
        {
            lstTag.Add(new ITagWord(tagName, obj, TagWordType.QRCode, string.Empty));
        }
        else if (document.Find(temp, false, true) != null)
        {
            string str = string.Empty;
            for (int a = 0, b = document.Sections.Count; a < b; a++)
            {
                for (int i = 0, n = document.Sections[a].Paragraphs.Count; i < n; i++)
                {
                    if (document.Sections[a].Paragraphs[i].Text.IndexOf(temp) != -1)
                    {
                        str += document.Sections[a].Paragraphs[i].Text;
                        int index = str.IndexOf(temp);
                        int endTag = str.Substring(index + temp.Count()).IndexOf(Contains.END_TAG);
                        string tag = str.Substring(index, endTag + index + temp.Count() + 1);
                        string style = tag.Substring(temp.Count() + 1).Replace(Contains.END_TAG, string.Empty);
                        document.Replace(tag, Contains.START_TAG + tagName + Contains.END_TAG, false, false);
                        lstTag.Add(new ITagWord(tagName, obj, TagWordType.Image, style));
                    }
                }
            }
        }
    }

    private void MergeCell(IWTable table, List<ListCellOfGroup> subGroup, int startIndex, int endIndex)
    {
        int columnGroupIndex = subGroup.First().IndexCell;
        string newCellText = GetCellText(table.Rows[startIndex].Cells[columnGroupIndex]);
        string oldCellText = newCellText;

        for (int i = startIndex; i < endIndex; i++)
        {
            newCellText = GetCellText(table.Rows[i].Cells[columnGroupIndex]);
            if (newCellText != oldCellText)
            {
                table.ApplyVerticalMerge(columnGroupIndex, startIndex, i - 1);
                oldCellText = newCellText;

                if (i - startIndex > 0 && subGroup.Count > 1)
                {
                    ListCellOfGroup[] subGroup1 = new ListCellOfGroup[subGroup.Count];
                    subGroup.CopyTo(subGroup1);
                    List<ListCellOfGroup> subGroup2 = subGroup1.ToList();
                    subGroup2.RemoveAt(0);
                    MergeCell(table, subGroup2, startIndex, i);
                }

                startIndex = i;
            }
            else if (i == endIndex - 1)
            {
                table.ApplyVerticalMerge(columnGroupIndex, startIndex, endIndex - 1);
                if (endIndex - startIndex > 0 && subGroup.Count > 1)
                {
                    ListCellOfGroup[] subGroup3 = new ListCellOfGroup[subGroup.Count];
                    subGroup.CopyTo(subGroup3);
                    List<ListCellOfGroup> subGroup4 = subGroup3.ToList();
                    subGroup4.RemoveAt(0);
                    MergeCell(table, subGroup4, startIndex, i + 1);
                }

                startIndex = i + 1;
            }
        }
    }

    private string GetCellText(WTableCell cell)
    {
        string text = string.Empty;
        foreach (WParagraph wp in cell.Paragraphs)
        {
            text += wp.Text.Trim();
        }

        return text;
    }
}
