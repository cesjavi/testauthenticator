using QRCoder;

namespace ItemManager.WebApp.Services;

public class QrCodeService
{
    public string GeneratePngDataUri(string content, int pixelsPerModule = 20)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("El contenido del código QR no puede estar vacío.", nameof(content));
        }

        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(data);
        var pngBytes = qrCode.GetGraphic(pixelsPerModule);
        var base64 = Convert.ToBase64String(pngBytes);
        return $"data:image/png;base64,{base64}";
    }
}
