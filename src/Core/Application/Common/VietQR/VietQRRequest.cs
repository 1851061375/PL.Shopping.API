using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace TD.WebApi.Application.Common.VietQR;

public class VietQRRequest
{
    [JsonProperty("orderCode")]
    public int OrderCode { get; set; }

    [JsonProperty("amount")]
    public int Amount { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("cancelUrl")]
    public string CancelUrl { get; set; }

    [JsonProperty("returnUrl")]
    public string ReturnUrl { get; set; }

    [JsonProperty("expiredAt")]
    public int ExpiredAt { get; set; }

    [JsonProperty("signature")]
    public string Signature { get; set; }

    public VietQRRequest(int orderCode, int amount, string description, string cancelUrl, string returnUrl, int expiredAt, string checkSumKey)
    {
        OrderCode = orderCode;
        Amount = amount;
        Description = description;
        CancelUrl = cancelUrl;
        ReturnUrl = returnUrl;
        ExpiredAt = expiredAt;
        Signature = GenerateSignature($"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}", checkSumKey);
    }
    private static string GenerateSignature(string input, string checkSumKey)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checkSumKey)))
        {
            byte[] signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(signatureBytes).Replace("-", string.Empty).ToLower();
        }
    }
}