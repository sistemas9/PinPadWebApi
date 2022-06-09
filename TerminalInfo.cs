using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PinPadWebApi
{
  public class TerminalInfo
  {
    public String EMVFull { get; set; }
    public String Modelo { get; set; }
    public String Serie { get; set; }
    public String Marca { get; set; }
    public String VersionApp { get; set; }
    public String COM { get; set; }
    public String Impresora { get; set; }
    public String VersionDLL { get; set; }
  }
}
