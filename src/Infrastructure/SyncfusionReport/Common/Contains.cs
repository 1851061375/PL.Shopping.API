using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD.WebApi.Infrastructure.SyncfusionReport.Common;
public class Contains
{
    public const string START_TAG = "{";
    public const string END_TAG = "}";
    public const string DATETIME_FORMAT = "dd/MM/yyyy";
    public const string GROUP_NODE = "Group:";
    public const string GROUP_ROW = "GroupRow:";
    public const char CHARACTER = ':';
    public const string SPLIT_NODE = "Split:";
    public const char EQUAL = '=';
    public const string PREFIX = "S";
    public const string JOIN = "_";
    public const string PREFIX_SHEET_FIRST = "S0_";
}