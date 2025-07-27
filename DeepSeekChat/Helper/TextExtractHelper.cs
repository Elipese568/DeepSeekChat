using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace DeepSeekChat.Helper;

public static class DocumentHelper
{
    // 获取文件的文本内容，依据外部传入的 MIME 类型
    public static string ExtractText(string mimeType, string filePath)
    {
        return mimeType switch
        {
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ExtractTextFromDocx(filePath),
            "application/vnd.openxmlformats-officedocument.presentationml.presentation" => ExtractTextFromPptx(filePath),
            "application/pdf" => ExtractTextFromPdf(filePath),
            "application/rtf" => ExtractTextFromRtf(filePath), // RTF 文件
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => ConvertXlsxToCsv(filePath),
            "application/vnd.oasis.opendocument.text" => ExtractTextFromOdt(filePath),  // ODT 文本文件
            "application/vnd.oasis.opendocument.spreadsheet" => ExtractTextFromOds(filePath), // ODS 电子表格
            "application/vnd.oasis.opendocument.presentation" => ExtractTextFromOdp(filePath), // ODP 演示文稿
            _ => File.ReadAllText(filePath),
        };
    }

    // 提取 .docx 文件的文本
    private static string ExtractTextFromDocx(string filePath)
    {
        StringBuilder text = new StringBuilder();
        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
        {
            foreach (var paragraph in wordDoc.MainDocumentPart.Document.Body.ChildElements.Where(x => x is Paragraph))
            {
                text.AppendLine(paragraph.InnerText);
            }
        }
        return text.ToString();
    }

    // 提取 .pptx 文件的文本
    private static string ExtractTextFromPptx(string filePath)
    {
        StringBuilder text = new StringBuilder();
        using (PresentationDocument pptxDoc = PresentationDocument.Open(filePath, false))
        {
            var slides = pptxDoc.PresentationPart.SlideParts;
            foreach (var slide in slides)
            {
                foreach (var textElement in slide.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                {
                    text.AppendLine(textElement.Text);
                }
            }
        }
        return text.ToString();
    }

    // 提取 .pdf 文件的文本
    private static string ExtractTextFromPdf(string filePath)
    {
        StringBuilder text = new StringBuilder();
        using (PdfDocument pdfDoc = PdfReader.Open(filePath, PdfDocumentOpenMode.ReadOnly))
        {
            for (int i = 0; i < pdfDoc.PageCount; i++)
            {
                var page = pdfDoc.Pages[i];
                // 提取每页的文本
                text.AppendLine(page.ToString());
            }
        }
        return text.ToString();
    }

    private static string ExtractTextFromRtf(string filePath)
    {
        string rtfText;
        using var reader = new StreamReader(filePath);

        // 使用 RtfPipe 库将 RTF 转换为纯文本
        var plainText = RichTextStripper.StripRichTextFormat(reader.ReadToEnd());
        return plainText;
    }

    private static string ConvertXlsxToCsv(string filePath)
    {
        StringBuilder sb = new StringBuilder();

        using (var document = SpreadsheetDocument.Open(filePath, false))
        {
            var workbookPart = document.WorkbookPart;
            var sheet = workbookPart.Workbook.Sheets.Elements<Sheet>().First();
            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);

            var rows = worksheetPart.Worksheet.GetFirstChild<SheetData>().Elements<Row>();

            foreach (var row in rows)
            {
                var cells = row.Elements<Cell>().Select(cell => GetCellValue(cell, workbookPart));
                sb.AppendLine(string.Join(",", cells));
            }
        }

        return sb.ToString();
    }

    private static string GetCellValue(Cell cell, WorkbookPart workbookPart)
    {
        string value = cell.InnerText;
        if (cell.DataType != null && cell.DataType == CellValues.SharedString)
        {
            var sharedStringItem = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(value));
            value = sharedStringItem.InnerText;
        }
        return value;
    }

    private static string ExtractTextFromOdt(string filePath)
    {
        StringBuilder text = new StringBuilder();

        using (Package package = Package.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            // 获取 content.xml 文件，OpenDocument 文件通常包含此文件
            Uri contentUri = new Uri("/content.xml", UriKind.Relative);
            PackagePart contentPart = package.GetPart(contentUri);

            // 解析 XML 文件
            XDocument doc = XDocument.Load(XmlReader.Create(contentPart.GetStream()));
            var paragraphs = doc.Descendants()
                                .Where(e => e.Name.LocalName == "p" || e.Name.LocalName == "text:p")
                                .Select(e => e.Value);

            // 拼接文本
            foreach (var paragraph in paragraphs)
            {
                text.AppendLine(paragraph.Trim());
            }
        }

        return text.ToString();
    }

    // 提取 ODS (OpenDocument Spreadsheet) 文件中的表格内容
    private static string ExtractTextFromOds(string filePath)
    {
        StringBuilder text = new StringBuilder();

        using (Package package = Package.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            // 获取 content.xml 文件
            Uri contentUri = new Uri("/content.xml", UriKind.Relative);
            PackagePart contentPart = package.GetPart(contentUri);

            // 解析 XML 文件
            XDocument doc = XDocument.Load(XmlReader.Create(contentPart.GetStream()));
            var rows = doc.Descendants()
                          .Where(e => e.Name.LocalName == "table:table-row")
                          .Select(row => row.Elements()
                                            .Where(cell => cell.Name.LocalName == "text:p")
                                            .Select(cell => cell.Value.Trim()));

            // 拼接每行的文本
            foreach (var row in rows)
            {
                text.AppendLine(string.Join(",", row));
            }
        }

        return text.ToString();
    }

    // 提取 ODP (OpenDocument Presentation) 文件中的文本内容
    private static string ExtractTextFromOdp(string filePath)
    {
        StringBuilder text = new StringBuilder();

        using (Package package = Package.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            // 获取 content.xml 文件
            Uri contentUri = new Uri("/content.xml", UriKind.Relative);
            PackagePart contentPart = package.GetPart(contentUri);

            // 解析 XML 文件
            XDocument doc = XDocument.Load(XmlReader.Create(contentPart.GetStream()));
            var textElements = doc.Descendants()
                                  .Where(e => e.Name.LocalName == "text:p")
                                  .Select(e => e.Value);

            // 拼接文本
            foreach (var textElement in textElements)
            {
                text.AppendLine(textElement.Trim());
            }
        }

        return text.ToString();
    }
}

/// <summary>
/// Rich Text Stripper
/// </summary>
/// <remarks>
/// Translated from Python located at:
/// http://stackoverflow.com/a/188877/448
/// </remarks>
public static class RichTextStripper
{
    private class StackEntry
    {
        public int NumberOfCharactersToSkip { get; set; }
        public bool Ignorable { get; set; }

        public StackEntry(int numberOfCharactersToSkip, bool ignorable)
        {
            NumberOfCharactersToSkip = numberOfCharactersToSkip;
            Ignorable = ignorable;
        }
    }

    private static readonly Regex _rtfRegex = new Regex(@"\\([a-z]{1,32})(-?\d{1,10})?[ ]?|\\'([0-9a-f]{2})|\\([^a-z])|([{}])|[\r\n]+|(.)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

    private static readonly List<string> destinations = new List<string>
    {
        "aftncn","aftnsep","aftnsepc","annotation","atnauthor","atndate","atnicn","atnid",
        "atnparent","atnref","atntime","atrfend","atrfstart","author","background",
        "bkmkend","bkmkstart","blipuid","buptim","category","colorschememapping",
        "colortbl","comment","company","creatim","datafield","datastore","defchp","defpap",
        "do","doccomm","docvar","dptxbxtext","ebcend","ebcstart","factoidname","falt",
        "fchars","ffdeftext","ffentrymcr","ffexitmcr","ffformat","ffhelptext","ffl",
        "ffname","ffstattext","field","file","filetbl","fldinst","fldrslt","fldtype",
        "fname","fontemb","fontfile","fonttbl","footer","footerf","footerl","footerr",
        "footnote","formfield","ftncn","ftnsep","ftnsepc","g","generator","gridtbl",
        "header","headerf","headerl","headerr","hl","hlfr","hlinkbase","hlloc","hlsrc",
        "hsv","htmltag","info","keycode","keywords","latentstyles","lchars","levelnumbers",
        "leveltext","lfolevel","linkval","list","listlevel","listname","listoverride",
        "listoverridetable","listpicture","liststylename","listtable","listtext",
        "lsdlockedexcept","macc","maccPr","mailmerge","maln","malnScr","manager","margPr",
        "mbar","mbarPr","mbaseJc","mbegChr","mborderBox","mborderBoxPr","mbox","mboxPr",
        "mchr","mcount","mctrlPr","md","mdeg","mdegHide","mden","mdiff","mdPr","me",
        "mendChr","meqArr","meqArrPr","mf","mfName","mfPr","mfunc","mfuncPr","mgroupChr",
        "mgroupChrPr","mgrow","mhideBot","mhideLeft","mhideRight","mhideTop","mhtmltag",
        "mlim","mlimloc","mlimlow","mlimlowPr","mlimupp","mlimuppPr","mm","mmaddfieldname",
        "mmath","mmathPict","mmathPr","mmaxdist","mmc","mmcJc","mmconnectstr",
        "mmconnectstrdata","mmcPr","mmcs","mmdatasource","mmheadersource","mmmailsubject",
        "mmodso","mmodsofilter","mmodsofldmpdata","mmodsomappedname","mmodsoname",
        "mmodsorecipdata","mmodsosort","mmodsosrc","mmodsotable","mmodsoudl",
        "mmodsoudldata","mmodsouniquetag","mmPr","mmquery","mmr","mnary","mnaryPr",
        "mnoBreak","mnum","mobjDist","moMath","moMathPara","moMathParaPr","mopEmu",
        "mphant","mphantPr","mplcHide","mpos","mr","mrad","mradPr","mrPr","msepChr",
        "mshow","mshp","msPre","msPrePr","msSub","msSubPr","msSubSup","msSubSupPr","msSup",
        "msSupPr","mstrikeBLTR","mstrikeH","mstrikeTLBR","mstrikeV","msub","msubHide",
        "msup","msupHide","mtransp","mtype","mvertJc","mvfmf","mvfml","mvtof","mvtol",
        "mzeroAsc","mzeroDesc","mzeroWid","nesttableprops","nextfile","nonesttables",
        "objalias","objclass","objdata","object","objname","objsect","objtime","oldcprops",
        "oldpprops","oldsprops","oldtprops","oleclsid","operator","panose","password",
        "passwordhash","pgp","pgptbl","picprop","pict","pn","pnseclvl","pntext","pntxta",
        "pntxtb","printim","private","propname","protend","protstart","protusertbl","pxe",
        "result","revtbl","revtim","rsidtbl","rxe","shp","shpgrp","shpinst",
        "shppict","shprslt","shptxt","sn","sp","staticval","stylesheet","subject","sv",
        "svb","tc","template","themedata","title","txe","ud","upr","userprops",
        "wgrffmtfilter","windowcaption","writereservation","writereservhash","xe","xform",
        "xmlattrname","xmlattrvalue","xmlclose","xmlname","xmlnstbl",
        "xmlopen"
    };

    private static readonly Dictionary<string, string> specialCharacters = new Dictionary<string, string>
    {
        { "par", "\n" },
        { "sect", "\n\n" },
        { "page", "\n\n" },
        { "line", "\n" },
        { "tab", "\t" },
        { "emdash", "\u2014" },
        { "endash", "\u2013" },
        { "emspace", "\u2003" },
        { "enspace", "\u2002" },
        { "qmspace", "\u2005" },
        { "bullet", "\u2022" },
        { "lquote", "\u2018" },
        { "rquote", "\u2019" },
        { "ldblquote", "\u201C" },
        { "rdblquote", "\u201D" },
    };
    /// <summary>
    /// Strip RTF Tags from RTF Text
    /// </summary>
    /// <param name="inputRtf">RTF formatted text</param>
    /// <returns>Plain text from RTF</returns>
    public static string StripRichTextFormat(string inputRtf)
    {
        if (inputRtf == null)
        {
            return null;
        }

        string returnString;

        var stack = new Stack<StackEntry>();
        bool ignorable = false;              // Whether this group (and all inside it) are "ignorable".
        int ucskip = 1;                      // Number of ASCII characters to skip after a unicode character.
        int curskip = 0;                     // Number of ASCII characters left to skip
        var outList = new List<string>();    // Output buffer.

        MatchCollection matches = _rtfRegex.Matches(inputRtf);

        if (matches.Count > 0)
        {
            foreach (Match match in matches)
            {
                string word = match.Groups[1].Value;
                string arg = match.Groups[2].Value;
                string hex = match.Groups[3].Value;
                string character = match.Groups[4].Value;
                string brace = match.Groups[5].Value;
                string tchar = match.Groups[6].Value;

                if (!String.IsNullOrEmpty(brace))
                {
                    curskip = 0;
                    if (brace == "{")
                    {
                        // Push state
                        stack.Push(new StackEntry(ucskip, ignorable));
                    }
                    else if (brace == "}")
                    {
                        // Pop state
                        StackEntry entry = stack.Pop();
                        ucskip = entry.NumberOfCharactersToSkip;
                        ignorable = entry.Ignorable;
                    }
                }
                else if (!String.IsNullOrEmpty(character)) // \x (not a letter)
                {
                    curskip = 0;
                    if (character == "~")
                    {
                        if (!ignorable)
                        {
                            outList.Add("\xA0");
                        }
                    }
                    else if ("{}\\".Contains(character))
                    {
                        if (!ignorable)
                        {
                            outList.Add(character);
                        }
                    }
                    else if (character == "*")
                    {
                        ignorable = true;
                    }
                }
                else if (!String.IsNullOrEmpty(word)) // \foo
                {
                    curskip = 0;
                    if (destinations.Contains(word))
                    {
                        ignorable = true;
                    }
                    else if (ignorable)
                    {
                    }
                    else if (specialCharacters.ContainsKey(word))
                    {
                        outList.Add(specialCharacters[word]);
                    }
                    else if (word == "uc")
                    {
                        ucskip = Int32.Parse(arg);
                    }
                    else if (word == "u")
                    {
                        int c = Int32.Parse(arg);
                        if (c < 0)
                        {
                            c += 0x10000;
                        }
                        outList.Add(Char.ConvertFromUtf32(c));
                        curskip = ucskip;
                    }
                }
                else if (!String.IsNullOrEmpty(hex)) // \'xx
                {
                    if (curskip > 0)
                    {
                        curskip -= 1;
                    }
                    else if (!ignorable)
                    {
                        int c = Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                        outList.Add(Char.ConvertFromUtf32(c));
                    }
                }
                else if (!String.IsNullOrEmpty(tchar))
                {
                    if (curskip > 0)
                    {
                        curskip -= 1;
                    }
                    else if (!ignorable)
                    {
                        outList.Add(tchar);
                    }
                }
            }
        }
        else
        {
            // Didn't match the regex
            returnString = inputRtf;
        }

        returnString = String.Join(String.Empty, outList.ToArray());

        return returnString;
    }
}