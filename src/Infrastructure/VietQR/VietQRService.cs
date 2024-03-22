using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QRCoder;
using TD.WebApi.Application.Catalog.AppConfigs;
using TD.WebApi.Application.Common.Persistence;
using TD.WebApi.Application.Common.VietQR;

namespace TD.WebApi.Infrastructure.VietQR;

public class VietQRService : IVietQRService
{
    private readonly VietQRSettings _settings;
    private readonly ILogger<VietQRService> _logger;
    private readonly IDapperRepository _repository;

    public VietQRService(IOptions<VietQRSettings> settings, ILogger<VietQRService> logger, IDapperRepository repository)
    {
        _settings = settings.Value;
        _logger = logger;
        _repository = repository;
    }

    private async Task<string> GetValueAppConfigs(string key)
    {
        var item = await _repository.QueryFirstOrDefaultObjectAsync<AppConfigDto>($"SELECT TOP (1) *  FROM [Catalog].[AppConfigs] WHERE [Key] = '{key}' AND DeletedOn IS NULL");
        if (item != null && !string.IsNullOrEmpty(item.Value))
        {
            return item.Value;
        }
        return string.Empty;
    }

    public async Task<CreateQRBankResponse>? GetVietQR(int orderCode, int amount, string description, string cancelUrl, string returnUrl, int expiredAt)
    {
        string checkSumKey = _settings.ChecksumKey;
        string clientId = _settings.ClientId;
        string apiKey = _settings.ApiKey;

        string itemConfigClientId = await GetValueAppConfigs("VietQRSettings_ClientId");
        if (!string.IsNullOrEmpty(itemConfigClientId))
        {
            clientId = itemConfigClientId;
        }

        string itemApiKey = await GetValueAppConfigs("VietQRSettings_APIKey");
        if (!string.IsNullOrEmpty(itemApiKey))
        {
            apiKey = itemApiKey;
        }

        string itemConfigCheckSumKey = await GetValueAppConfigs("VietQRSettings_ChecksumKey");
        if (!string.IsNullOrEmpty(itemConfigCheckSumKey))
        {
            checkSumKey = itemConfigCheckSumKey;
        }

        var requestData = new VietQRRequest(orderCode, amount, description, cancelUrl, returnUrl, expiredAt, checkSumKey);

        var client = new HttpClient();
        var requesthttp = new HttpRequestMessage(HttpMethod.Post, "https://api-merchant.payos.vn/v2/payment-requests");
        requesthttp.Headers.Add("x-client-id", clientId);
        requesthttp.Headers.Add("x-api-key", apiKey);

        requesthttp.Content = new StringContent(JsonConvert.SerializeObject(requestData), null, "application/json");
        var response = await client.SendAsync(requesthttp);
        response.EnsureSuccessStatusCode();

        string result = await response.Content.ReadAsStringAsync();

        VietQRResponse? deserializedProduct = JsonConvert.DeserializeObject<VietQRResponse>(result);

        if (deserializedProduct != null && deserializedProduct.Data != null)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(deserializedProduct.Data.QrCode, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);

            DateTimeOffset now = (DateTimeOffset)DateTime.Now;
            return new CreateQRBankResponse() { QRCodeImage = Convert.ToBase64String(qrCodeAsPngByteArr), QRCodeData = deserializedProduct.Data.QrCode, TimeStamp = now.ToUnixTimeSeconds().ToString(), CheckoutUrl = deserializedProduct.Data.CheckoutUrl };
        }

        return null;
    }
}