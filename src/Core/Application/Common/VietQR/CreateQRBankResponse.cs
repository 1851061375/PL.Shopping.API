using Newtonsoft.Json;
using QRCoder;

namespace TD.WebApi.Application.Common.VietQR;

public class CreateQRBankResponse : IDto
{
    public string? QRCodeImage { get; set; }
    public string? QRCodeData { get; set; }
    public string? CheckoutUrl { get; set; }
    public string? TimeStamp { get; set; }
}
