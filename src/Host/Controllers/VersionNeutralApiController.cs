﻿using Asp.Versioning;

namespace TD.WebApi.Host.Controllers;

[Route("api/[controller]")]
[ApiVersionNeutral]
public class VersionNeutralApiController : BaseApiController
{
}
