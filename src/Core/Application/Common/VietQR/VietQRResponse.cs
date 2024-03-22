using Newtonsoft.Json;

namespace TD.WebApi.Application.Common.VietQR;

public class VietQRResponse
{
    [JsonProperty("code")]
    public string? Code { get; set; }

    [JsonProperty("desc")]
    public string? Desc { get; set; }

    [JsonProperty("data")]
    public DataVietQR? Data { get; set; }

    [JsonProperty("signature")]
    public string? Signature { get; set; }
}

public class DataVietQR
{
    [JsonProperty("bin")]
    public string? Bin { get; set; }

    [JsonProperty("accountNumber")]
    public string? AccountNumber { get; set; }

    [JsonProperty("accountName")]
    public string? AccountName { get; set; }

    [JsonProperty("amount")]
    public int? Amount { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("orderCode")]
    public int? OrderCode { get; set; }

    [JsonProperty("paymentLinkId")]
    public string? PaymentLinkId { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; }

    [JsonProperty("expiredAt")]
    public int? ExpiredAt { get; set; }

    [JsonProperty("checkoutUrl")]
    public string? CheckoutUrl { get; set; }

    [JsonProperty("qrCode")]
    public string? QrCode { get; set; }
}