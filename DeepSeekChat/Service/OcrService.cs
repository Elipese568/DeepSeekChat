using DocumentFormat.OpenXml.Vml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Ocr;
using Windows.Storage;

namespace DeepSeekChat.Service;

public class OcrServiceException : Exception
{
    public OcrServiceException(string message) : base(message) { }
}

public class OcrService
{
    private readonly OcrEngine _ocrEngine;
    public OcrService()
    {
        if(OcrEngine.AvailableRecognizerLanguages.Count == 0)
        {
            throw new OcrServiceException("No OCR languages available. Please install the necessary language packs.");
        }
        _ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages() ?? OcrEngine.TryCreateFromLanguage(OcrEngine.AvailableRecognizerLanguages[0]);
    }

    public async Task<string> DelectTextAsync(StorageFile imageFile)
    {
        using (var inStream = await imageFile.OpenReadAsync())
        {
            // 解码图片
            var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(inStream);
            // 获取图像
            var swbmp = await decoder.GetSoftwareBitmapAsync();
            var result = await _ocrEngine.RecognizeAsync(swbmp);
            return result.Text;
        }
    }
}
