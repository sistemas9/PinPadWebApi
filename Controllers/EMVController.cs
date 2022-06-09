using cpIntegracionEMV;
using Microsoft.AspNetCore.Mvc;
using PinPadWebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using PinPadWebApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace PinPadWebApi
{
  [ApiController]
  [Route("[controller]")]
  [GuidAttribute("a86a5dae-0771-489b-91d1-be3ac24d2d32")]
  [ComVisible(true)]
  public class EMVController : ControllerBase
  {
    private IConfiguration _configuration;
    public static clsCpIntegracionEMV cpIntegraEMV = new clsCpIntegracionEMV();

    private static string _user;
    private static string _pass;
    private static string _usrTrax;
    private static string _company;
    private static string _branch;
    private static string _country;
    private static string _url;
    private static string _tabSelected;
    private static string _resProductos;
    private static bool _imprimeLeyenda;
    private static int _tabGeneral;
    private static int _ini = 0;
    private static string _pagomVMC;
    private static string _pagomAMEX;
    private static string _pagobVMC;
    private static string _pagobAMEX;
    private static string _pagobSIP;
    private static bool _FacturaE;
    private static bool _Points2;
    private string numeroOperacion = "";
    public static bool Cerrar;

    private static string _paynopain;
    private static string csvAmexenBanda;

    public static string User
    {
      get { return _user; }
      set { _user = value; }
    }

    public static string Pass
    {
      get { return _pass; }
      set { _pass = value; }
    }

    public static string UsrTrx
    {
      get { return _usrTrax; }
      set { _usrTrax = value; }
    }

    public static string Company
    {
      get { return _company; }
      set { _company = value; }
    }

    public static string Branch
    {
      get { return _branch; }
      set { _branch = value; }
    }

    public static string Country
    {
      get { return _country; }
      set { _country = value; }
    }

    public static string Url
    {
      get { return _url; }
      set { _url = value; }
    }

    public static string tabSelected
    {
      get { return _tabSelected; }
      set { _tabSelected = value; }
    }

    public static string resProductos
    {
      get { return _resProductos; }
      set { _resProductos = value; }
    }

    public static bool imprimeLeyenda
    {
      get { return _imprimeLeyenda; }
      set { _imprimeLeyenda = value; }
    }

    public static int tabGeneral
    {
      get { return _tabGeneral; }
      set { _tabGeneral = value; }
    }

    public static int ini
    {
      get { return _ini; }
      set { _ini = value; }
    }

    public static string pagomVMC
    {
      get { return _pagomVMC; }
      set { _pagomVMC = value; }
    }

    public static string pagomAMEX
    {
      get { return _pagomAMEX; }
      set { _pagomAMEX = value; }
    }

    public static string pagobVMC
    {
      get { return _pagobVMC; }
      set { _pagobVMC = value; }
    }

    public static string pagobAMEX
    {
      get { return _pagobAMEX; }
      set { _pagobAMEX = value; }
    }

    public static string pagobSIP
    {
      get { return _pagobSIP; }
      set { _pagobSIP = value; }
    }

    public static bool FacturaE
    {
      get { return _FacturaE; }
      set { _FacturaE = value; }
    }

    public static bool Points2
    {
      get { return _Points2; }
      set { _Points2 = value; }
    }

    //PayNoPain
    public static bool PayNoPain { get; set; }

    //Tiempo Aire
    public static string CSVAmexenBanda
    {
      get { return csvAmexenBanda; }
      set { csvAmexenBanda = value; }
    }


    //URL keyWeb
    public static string URL_PublicKey { get; set; }

    //Info DCC
    public static string infoDCC { get; set; }
    public static string TypeDCC { get; set; }
    public static string TxAmount { get; set; }

    //manejo de moneda
    public static bool HidePPMoneda = false;
    public static bool HidePPMerchant = false;
    private string descripcionMoneda = "";

    //Bases de datos
    private DatabaseExport dbexport = new DatabaseExport();
    private Database dbprod = new Database();

    public Boolean cancel = false;

    public EMVController(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    [HttpGet("login")]
    [ComVisible(true)]
    public bool login()
    {
      SerialPort port = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
      String User = _configuration["UserSetting"];
      String Password = _configuration["PasswordSetting"];
      byte[] UserData = Convert.FromBase64String(User);
      User = Encoding.UTF8.GetString(UserData);
      byte[] PasswordData = Convert.FromBase64String(Password);
      Password = Encoding.UTF8.GetString(PasswordData);
      ////////////////////////nuevo///////
      cpIntegraEMV.ActivateSandbox(false);
      URL_PublicKey = "https://qa10.mitec.com.mx";
      Url = "https://qa3.mitec.com.mx";
      //Url = Program.Url;
      cpIntegraEMV.dbgSetUrl(Url);
      cpIntegraEMV.dbgSetActivateMagtek(true);
      var login = cpIntegraEMV.dbgLoginUser(User, Password);
      cpIntegraEMV.dbgSetUrlIpKeyWeb(URL_PublicKey);
      cpIntegraEMV.ObtieneLlavePublicaRSA();

      if (cpIntegraEMV.getRespPublicKeyRSA().ToUpper().Equals("FALSE"))
      {
        return false;
      }
      if (!cancel)
      {
        Response.Headers.Add("Access-Control-Allow-Origin", "*");
      }
      return login;
      ////////////////////////////////////
    }

    [HttpGet("GetTerminalInfo")]
    [ComVisible(true)]
    public TerminalInfo GetTerminalInfo()
    {
      TerminalInfo info = new TerminalInfo();
      info.EMVFull = cpIntegraEMV.chkPp_EMVFull();
      info.Modelo = cpIntegraEMV.chkPp_Model();
      info.Serie = cpIntegraEMV.chkPp_Serial();
      info.Marca = cpIntegraEMV.chkPp_Trademark();
      info.VersionApp = cpIntegraEMV.chkPp_Version();
      info.COM = cpIntegraEMV.chkPp_Com();
      info.Impresora = cpIntegraEMV.chkPp_Printer();
      info.VersionDLL = cpIntegraEMV.dbgGetVersion();
      setValues();
      String giro = cpIntegraEMV.dbgGetGiro();
      if (!cancel)
      {
        Response.Headers.Add("Access-Control-Allow-Origin", "*");
      }
      return info;
    }

    [HttpGet("GetCompanyInfo")]
    public CompanyInfo GetCompanyInfo()
    {
      CompanyInfo compInfo = new CompanyInfo();
      compInfo.bs_user = cpIntegraEMV.dbgGetUser();
      compInfo.bs_pswd = cpIntegraEMV.dbgGetPass();
      compInfo.bs_company = cpIntegraEMV.dbgGetId_Company();
      compInfo.bs_branch = cpIntegraEMV.dbgGetId_Branch();
      compInfo.bs_country = cpIntegraEMV.dbgGetCountry();
      compInfo.nb_user = cpIntegraEMV.dbgGetNb_User();
      compInfo.nb_company = cpIntegraEMV.dbgGetNb_Company();
      compInfo.nb_street = cpIntegraEMV.dbgGetNb_companystreet();
      compInfo.nb_branch = cpIntegraEMV.dbgGetNb_Branch();
      compInfo.bs_usrtrx = UsrTrx;
      compInfo.dbgseturl = Url;
      if (!cancel)
      {
        Response.Headers.Add("Access-Control-Allow-Origin", "*");
      }
      return compInfo;
    }

    [HttpGet("LeerTarjeta")]
    public async Task<DatosTarjeta> LeerTarjeta(String amount)
    {
      var re = Request;
      var headers = re.Headers;
      String mesa = String.Empty;
      Response.Headers.Add("Access-Control-Allow-Origin", "*");
      cpIntegraEMV.dbgSetActivateMagtek(true);
      cpIntegraEMV.dbgSetUrl(Url);
      //se oculta el popup de la moneda
      cpIntegraEMV.dbgHidePopUp(HidePPMoneda);
      //llena el tipo de moneda con el que se puede pagar
      if (!HidePPMoneda)
        cpIntegraEMV.EstableceTipoMoneda(cpIntegraEMV.ObtieneMonedaVtaPropiaBanda());
      else
        cpIntegraEMV.dbgSetCurrency("MXN");
      cpIntegraEMV.dbgStartTxEMV(amount);
      if (cpIntegraEMV.chkPp_CdError() == "")
      {
        DatosTarjeta tarj = new DatosTarjeta();
        tarj.NumeroTdc = cpIntegraEMV.chkCc_Number();
        tarj.NombreTdc = cpIntegraEMV.chkCc_Name();
        tarj.Mes = cpIntegraEMV.chkCc_ExpMonth();
        tarj.Anio = cpIntegraEMV.chkCc_ExpYear();
        tarj.ErrorMessage = "Sin error";
        var serializeTarj = JsonConvert.SerializeObject(tarj);
        byte[] bytes = Encoding.ASCII.GetBytes(serializeTarj);
        return tarj;
      }
      else
      {
        mesa = " DescError : " + cpIntegraEMV.chkPp_CdError() + " CodError :  " + cpIntegraEMV.chkPp_DsError();
      }
      return new DatosTarjeta() { ErrorMessage = mesa } ;
    }

    [HttpGet("EjecutaCobro")]
    public DatosVoucher EjecutaCobro(String refer,String origen, String amount)
    {
      Response.Headers.Add("Access-Control-Allow-Origin", "*");
      //String refer = Request.Form["refer"];
      //String origen = Request.Form["origen"];
      DatosVoucher result = new DatosVoucher();
      cpIntegraEMV.dbgSetActivateMagtek(true);
      cpIntegraEMV.dbgSetUrl("https://qa3.mitec.com.mx");
      cpIntegraEMV.dbgSetUrlIpKeyWeb("https://qa10.mitec.com.mx");
      if (HidePPMoneda)
      {
        descripcionMoneda = "MXN";
      }
      else
      {
        if (cpIntegraEMV.GetTipoMoneda() != "")
          descripcionMoneda = cpIntegraEMV.GetTipoMoneda();
        else
          descripcionMoneda = "MXN";
      }

      String txtOperType = "11";
      String cbxType = "V/MC";
      String txtRef = refer;
      String cvvAmex = "";
      String response = "";
      String parametros = "";
      String UsrTrx = "URSTRX";
      User = cpIntegraEMV.dbgGetUser();
      Pass = cpIntegraEMV.dbgGetPass();
      Company = cpIntegraEMV.dbgGetId_Company();
      Branch = cpIntegraEMV.dbgGetId_Branch();
      Country = cpIntegraEMV.dbgGetCountry();
      bool dcc = cpIntegraEMV.aplicaDCC();
      String txtMerchant = cpIntegraEMV.dbgGetMerchantBanda(txtOperType);

      cpIntegraEMV.sndVtaDirectaEMV(User, Pass, UsrTrx, Company, Branch, Country, cbxType, txtMerchant, txtRef, txtOperType, descripcionMoneda, cvvAmex);
      ////paso 9
      switch (cpIntegraEMV.getRspDsResponse())
      {
        case "approved":
          response = "approved";

          String NOperacion = cpIntegraEMV.getRspOperationNumber();
          String NAutorizacion = cpIntegraEMV.getRspAuth();
          String CDResp = cpIntegraEMV.getRspCdResponse();
          String Ref = cpIntegraEMV.getTx_Reference();
          String monto = amount;
          String Arqc = cpIntegraEMV.getRspArqc();
          String Aid = cpIntegraEMV.getRspAppid();
          String AidLabel = cpIntegraEMV.getRspAppidlabel();
          String origenTrx = origen;// _configuration["environment"];

          parametros = cpIntegraEMV.getRspDsResponse() +
              " NOperacion = " + NOperacion +
              " NAutorizacion = " + NAutorizacion +
              " CDResp = " + CDResp +
              " Ref = " + Ref +
              " Arqc = " + Arqc +
              " Aid = " + Aid +
              " AidLabel = " + AidLabel;

          String voucher2 = cpIntegraEMV.getRspVoucher();

          /////////////////log de pinpad DB ayt_03//////////////////////////////
          DatabaseAYT03 ayt03_log = new DatabaseAYT03();
          List<parametrosSP> paramsSP = new List<parametrosSP>();
          parametrosSP parm = new parametrosSP();
          parametrosSP parm1 = new parametrosSP();
          parametrosSP parm2 = new parametrosSP();
          parametrosSP parm3 = new parametrosSP();
          parametrosSP parm4 = new parametrosSP();
          parametrosSP parm5 = new parametrosSP();
          parametrosSP parm6 = new parametrosSP();
          parametrosSP parm7 = new parametrosSP();
          parametrosSP parm8 = new parametrosSP();
          parametrosSP parm9 = new parametrosSP();

          parm.nombre = "@NOperacion";
          parm1.nombre = "@NAutorizacion";
          parm2.nombre = "@CDResp";
          parm3.nombre = "@Ref";
          parm4.nombre = "@monto";
          parm5.nombre = "@Arqc";
          parm6.nombre = "@Aid";
          parm7.nombre = "@AidLabel";
          parm8.nombre = "@voucher";
          parm9.nombre = "@origen";

          parm.valor = NOperacion;
          parm1.valor = NAutorizacion;
          parm2.valor = CDResp;
          parm3.valor = Ref;
          parm4.valor = monto;
          parm5.valor = Arqc;
          parm6.valor = Aid;
          parm7.valor = AidLabel;
          parm8.valor = voucher2;
          parm9.valor = origenTrx;

          paramsSP.Add(parm);
          paramsSP.Add(parm1);
          paramsSP.Add(parm2);
          paramsSP.Add(parm3);
          paramsSP.Add(parm4);
          paramsSP.Add(parm5);
          paramsSP.Add(parm6);
          paramsSP.Add(parm7);
          paramsSP.Add(parm8);
          paramsSP.Add(parm9);

          int rows = ayt03_log.QueryStroredProcedure("AYT_InsertPinPadLogTransaction", paramsSP).Result;
          /////////////////////////////////////////////////////////////////////

          imprVoucher();
          result.voucher = voucher2;

          //******************************************************************************************
          //Se solicita la Firma en Panel
          StringBuilder textoAgua = new StringBuilder();
          textoAgua.Append("Folio: " + cpIntegraEMV.getRspOperationNumber());
          textoAgua.Append("\nAuth: " + cpIntegraEMV.getRspAuth());
          textoAgua.Append("\nImporte: " + cpIntegraEMV.getTx_Amount());
          textoAgua.Append("\nFecha: " + cpIntegraEMV.getRspDate());
          textoAgua.Append("\nMerchant: " + cpIntegraEMV.getRspDsMerchant());
          textoAgua.Append("\n ");
          textoAgua.Append("\n___________________");
          textoAgua.Append("\nFIRMA DIGITALIZADA:");

          bool IsChipAndPin = false;
          bool esQPS = false;
          string cadenaHexFirma = string.Empty;
          string strHexFirmaPanel = string.Empty;
          string ipFirmaPanel = Url;

          if (cpIntegraEMV.chkCc_IsPin().Equals("01"))
            IsChipAndPin = true;

          if (cpIntegraEMV.getRspVoucher().Contains("@cnn Autorizado sin Firma ") && !IsChipAndPin)
            esQPS = true;

          //Se detecta el dispositivo desde donde se ejecuta PcPay
          bool isTouch = isTouchTerminal();
          //var thread2 = new Thread(new ThreadStart(cpIntegraEMV.EsTouch));
          //thread2.Start();
          if (isTouch)
          {
            //Se llama el Form para la firma
            cpIntegraEMV.ObtieneFirmaPanel(textoAgua.ToString());

            //Se valida si hay error
            //Si no hay error, se obtiene la cadena string de la firma digital
            if (string.IsNullOrEmpty(cpIntegraEMV.Error()))
            {
              strHexFirmaPanel = cpIntegraEMV.TextoHEXFirmaPanel();

              //Se guarda la firma en base de datos
              numeroOperacion = cpIntegraEMV.getRspOperationNumber();
              if (cpIntegraEMV.sndGuardaFirmaPanel(strHexFirmaPanel, ipFirmaPanel, numeroOperacion, cpIntegraEMV.chkPp_Serial()))
              {
                //txtMail.Visible = true;
                //lblMail.Visible = true;
                //btnMail.Visible = true;
                //this.chbMailAutomatico.Visible = false;

              }
              else
              {
                //MessageBox.Show("No se pudo guardar la firma correctamente");
                result.parametros = "No se pudo guardar la firma correctamente";
                result.status = "error";
              }
            }
            else
            {
              //MessageBox.Show("No se pudo obtener la firma desde el dispositivo" + Program.cpIntegraEMV.Error());
              result.parametros = "No se pudo obtener la firma desde el dispositivo" + cpIntegraEMV.Error();
              result.status = "error";
            }


          }
          else
          {
            if (cpIntegraEMV.getRspSoportaFirmaPanel())
            {

              //se pide la firma en la PinPad
              strHexFirmaPanel = cpIntegraEMV.sndObtieneFirmaPanel(false, textoAgua.ToString(), cpIntegraEMV.getRspVoucher(),
                                                                           IsChipAndPin, cpIntegraEMV.chkPp_Trademark(), 1, esQPS);

              //Se valida si hay error
              //Si no hay error, se obtiene la cadena string de la firma digital
              if (!strHexFirmaPanel.Contains("Error"))
              {
                //Se guarda la firma en base de datos
                numeroOperacion = cpIntegraEMV.getRspOperationNumber();
                if (cpIntegraEMV.sndGuardaFirmaPanel(strHexFirmaPanel, ipFirmaPanel, numeroOperacion, cpIntegraEMV.chkPp_Serial()))
                {

                  //txtMail.Visible = true;
                  //lblMail.Visible = true;
                  //btnMail.Visible = true;
                  //chbMailAutomatico.Visible = true;
                  //sendMail("sistemas9@avanceytec.com.mx"); // aqui se envia el correo cuando la terminal no tiene impresora
                }
                else
                {
                  //MessageBox.Show("No se pudo guardar la firma correctamente");
                  result.parametros = "No se pudo guardar la firma correctamente";
                  result.status = "error";
                }
              }
              else
              {
                //MessageBox.Show("No se pudo obtener la firma desde el dispositivo" + Program.cpIntegraEMV.Error());
                result.parametros = "No se pudo obtener la firma desde el dispositivo" + cpIntegraEMV.Error();
                result.status = "error";
              }
            }
            else
            {
              // MessageBox.Show("El dispositivo NO soporta firma en panel");
            }

          }

          result.status = "approved";
          result.parametros = parametros;
          //guarda en base de datos
          String query = "INSERT INTO [dbo].[AYT_PinPadLog] ([numeroOperacion],[numeroAutorizacion],[cdResp],[reference],[monto],[arqC],[aid],[aidLabel],[voucher],[fechaCreacion]) " +
                          "VALUES ('" + NOperacion + "','" + NAutorizacion + "','" + CDResp + "','" + Ref + "','" + monto + "','"+
                          "','" + Arqc + "','" + Aid + "','" + AidLabel + "','" + voucher2 +
                          "',CONVERT(DATETIME,SYSDATETIMEOFFSET() AT TIME ZONE 'Mountain Standard Time (Mexico)'));";
          if (origen == "DESARROLLO")
          {
            result.rowsAffected = dbprod.QueryInsert(query).Result;
          }
          else
          {
            result.rowsAffected = dbexport.QueryInsert(query).Result;
          }
          break;
        case "error":
          response = cpIntegraEMV.getRspDsResponse() + " DescError = " + cpIntegraEMV.getRspDsError() +
          " CodError =  " + cpIntegraEMV.getRspCdError() + "\r\n" + cpIntegraEMV.getRspFriendlyResponse();
          result.voucher = "";
          result.status = "error";
          result.parametros = response;
          break;
        case "denied":
          response = cpIntegraEMV.getRspDsResponse() + " NOperacion = " + cpIntegraEMV.getRspOperationNumber() +
          "\r\n CDResp = " + cpIntegraEMV.getRspCdResponse() +
          " Ref = " + cpIntegraEMV.getTx_Reference() + " " + cpIntegraEMV.getRspFriendlyResponse();
          result.voucher = "";
          result.status = "denied";
          result.parametros = response;
          break;
      }
      cpIntegraEMV.dbgEndOperation();
      cpIntegraEMV.dbgClearDCC();
      return result;
    }

    [HttpGet("imprVoucher")]
    public void imprVoucher()
    {
      String VComercio = String.Empty;
      String VCliente = String.Empty;
      if (cpIntegraEMV.getRspVoucherCliente() != "")
      {
        VComercio = cpIntegraEMV.getRspVoucherComercio();
        VCliente = cpIntegraEMV.getRspVoucherCliente() + cpIntegraEMV.getRspFeTxLeyenda(); //(Este parametro se usa solamente si se requiere factura electronica)
      }
      VComercio = VComercio.Substring(0, VComercio.IndexOf("@lsn POR ESTE"));
      VComercio += "\n@br\n\n\n@cnb AVANCE Y TECNOLOGIA\n@br\n@cnb EN PLASTICOS\n@br\n\n\n@br\n";
      VCliente = VCliente.Substring(0, VCliente.IndexOf("@lsn POR ESTE"));
      VCliente += "\n@br\n\n\n@br\n@cnb AVANCE Y TECNOLOGIA\n@br\n@cnb EN PLASTICOS\n@br\n\n\n@br\n";
      if (cpIntegraEMV.chkPp_Printer() == "1")
      {
        cpIntegraEMV.dbgPrint(VComercio);
        cpIntegraEMV.dbgPrint(VCliente);
      }
    }

    private void setValues()
    {
      User = cpIntegraEMV.dbgGetUser();
      Pass = cpIntegraEMV.dbgGetPass();
      Company = cpIntegraEMV.dbgGetId_Company();
      Branch = cpIntegraEMV.dbgGetId_Branch();
      Country = cpIntegraEMV.dbgGetCountry();
      cpIntegraEMV.dbgSetUrl(Url);
      resProductos = cpIntegraEMV.dbgGetResProductos();
      UsrTrx = "URSTRX";

      //pendiente
      //Program.PPOperacion.dbgSetUrl(URL);

      System.Diagnostics.FileVersionInfo verI =
      System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
      cpIntegraEMV.dbgSetTimeOut("5");
      pagomVMC = cpIntegraEMV.dbgGetpagomVMC();
      pagomAMEX = cpIntegraEMV.dbgGetpagomAMEX();
      pagobVMC = cpIntegraEMV.dbgGetpagobVMC();
      pagobAMEX = cpIntegraEMV.dbgGetpagobAMEX();
      pagobSIP = cpIntegraEMV.dbgGetpagobSIP();
      FacturaE = cpIntegraEMV.dbgGetFacturaE();
      Points2 = cpIntegraEMV.dbgGetPoints2();

      //PayNoPain
      PayNoPain = cpIntegraEMV.dbgGetActivaCupones();

      //Tiempo Aire
      CSVAmexenBanda = "";
      cpIntegraEMV.dbgSetTrxData(User, Pass, UsrTrx, Company, Branch, Country);
    }

    [HttpGet("getDisplayMessages")]
    public string getDisplayMessages()
    {
      String Msg = cpIntegraEMV.dbgGetDisplay();
      Response.Headers.Add("Access-Control-Allow-Origin", "*");
      return JsonConvert.SerializeObject(Msg);
    }

    public Boolean isTouchTerminal()
    {
      //bool isTouch = cpIntegraEMV.EsTouch();
      bool isTouch = false;
      //Thread t = new Thread(new ThreadStart( () =>
      //{
      //    isTouch = cpIntegraEMV.EsTouch();
      //}
      //));
      //t.Start();
      return isTouch;
    }

    [HttpGet("sendMail")]
    public Boolean sendMail(String correo)
    {
      string strMail = correo;
      string ipFirmaPanel = Url;
      numeroOperacion = cpIntegraEMV.getRspOperationNumber();
      String company = cpIntegraEMV.dbgGetId_Company();
      String branch = cpIntegraEMV.dbgGetId_Branch();
      String country = cpIntegraEMV.dbgGetCountry();
      String user = cpIntegraEMV.dbgGetUser();
      String password = cpIntegraEMV.dbgGetPass();

      if (cpIntegraEMV.sndEnviaMailFirmaPanel(strMail, numeroOperacion, company,
                                                     branch, country, user,
                                                     password, ipFirmaPanel))
      {
        //if (this.chbMailAutomatico.Checked)
        //    Program.cpIntegraEMV.ObtieneFormMail("0", string.Empty);
        //else
        //{
        //    MessageBox.Show("correo electrónico enviado correctamente");
        //    this.txtMail.Text = string.Empty;
        //}
        return true;
      }
      else
        //MessageBox.Show("correo electrónico no se pudo enviar correctamente");
        return false;


    }

    public void CerrarTrx()
    {
      cpIntegraEMV.dbgEndOperation();
      cpIntegraEMV.dbgClearDCC();
    }

    [HttpGet("Estado")]
    public String Estado()
    {
      Response.Headers.Add("Access-Control-Allow-Origin", "*");
      return "OnLine";
    }

    [HttpGet("Cancel")]
    public String Cancel()
    {
      Response.Headers.Add("Access-Control-Allow-Origin", "*");
      cpIntegraEMV.dbgCancelOperation();
      return JsonConvert.SerializeObject("Lectura Cancelada");
    }

    [HttpGet("cancelTransaction")]
    public DatosVoucher CancelTransaction(String amount, String operationNumber, String auth, String idLog)
    {
      DatosVoucher result = new DatosVoucher();
      try
      {
        cancel = true;
        Boolean loggedIn = login();
        String parametros = String.Empty;
        String response = "";
        if (loggedIn)
        {
          TerminalInfo terminal = GetTerminalInfo();
          if (terminal.COM != "")
          {
            cpIntegraEMV.sndCancelacion(User, Pass, UsrTrx, Company, Branch, Country, amount.Replace(",", ""), operationNumber, auth);
            String NOperacion = cpIntegraEMV.getRspOperationNumber();
            String NAutorizacion = cpIntegraEMV.getRspAuth();
            String CDResp = cpIntegraEMV.getRspCdResponse();
            String Ref = cpIntegraEMV.getTx_Reference();
            String monto = amount;
            String Arqc = cpIntegraEMV.getRspArqc();
            String Aid = cpIntegraEMV.getRspAppid();
            String AidLabel = cpIntegraEMV.getRspAppidlabel();
            String origenTrx = _configuration["environment"];

            parametros = cpIntegraEMV.getRspDsResponse() +
              " NOperacion = " + NOperacion +
              " NAutorizacion = " + NAutorizacion +
              " CDResp = " + CDResp +
              " Ref = " + Ref +
              " Arqc = " + Arqc +
              " Aid = " + Aid +
              " AidLabel = " + AidLabel;

            switch (cpIntegraEMV.getRspDsResponse())
            {
              case "approved":
                imprVoucher();
                result.status = "approved";
                DatabaseAYT03 ayt03_cancel = new DatabaseAYT03();
                String updateCanceledTicket = "UPDATE AYT_PinpadLog SET status = 1 WHERE idLog = " + idLog;
                int rowsAffected = ayt03_cancel.QueryInsert(updateCanceledTicket).Result;
                break;
              case "error":
                response = cpIntegraEMV.getRspDsResponse() + " DescError = " + cpIntegraEMV.getRspDsError() +
                " CodError =  " + cpIntegraEMV.getRspCdError() + "\r\n" + cpIntegraEMV.getRspFriendlyResponse();
                result.voucher = "";
                result.status = "error";
                result.parametros = response;
                break;
              case "denied":
                response = cpIntegraEMV.getRspDsResponse() + " NOperacion = " + cpIntegraEMV.getRspOperationNumber() +
                "\r\n CDResp = " + cpIntegraEMV.getRspCdResponse() +
                " Ref = " + cpIntegraEMV.getTx_Reference() + " " + cpIntegraEMV.getRspFriendlyResponse();
                result.voucher = "";
                result.status = "denied";
                result.parametros = response;
                break;
            }
          }
        }
      }catch(Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      Response.Headers.Add("Access-Control-Allow-Origin", "*");
      return result;
    }

    [HttpGet("reimprimirTransaction")]
    public DatosVoucher ReimprimirTransaction(String amount, String operationNumber, String auth, String idLog)
    {
      DatosVoucher result = new DatosVoucher();
      try
      {
        cancel = true;
        Boolean loggedIn = login();
        String parametros = String.Empty;
        String response = "";
        if (loggedIn)
        {
          TerminalInfo terminal = GetTerminalInfo();
          if (terminal.COM != "")
          {
            cpIntegraEMV.sndReimpresion(User, Pass, Company, Branch, Country, operationNumber);
            String NOperacion = cpIntegraEMV.getRspOperationNumber();
            String NAutorizacion = cpIntegraEMV.getRspAuth();
            String CDResp = cpIntegraEMV.getRspCdResponse();
            String Ref = cpIntegraEMV.getTx_Reference();
            String monto = amount;
            String Arqc = cpIntegraEMV.getRspArqc();
            String Aid = cpIntegraEMV.getRspAppid();
            String AidLabel = cpIntegraEMV.getRspAppidlabel();
            String origenTrx = _configuration["environment"];

            parametros = cpIntegraEMV.getRspDsResponse() +
              " NOperacion = " + NOperacion +
              " NAutorizacion = " + NAutorizacion +
              " CDResp = " + CDResp +
              " Ref = " + Ref +
              " Arqc = " + Arqc +
              " Aid = " + Aid +
              " AidLabel = " + AidLabel;

            switch (cpIntegraEMV.getRspDsResponse())
            {
              case "approved":
                imprVoucher();
                result.status = "approved";
                break;
              case "error":
                response = cpIntegraEMV.getRspDsResponse() + " DescError = " + cpIntegraEMV.getRspDsError() +
                " CodError =  " + cpIntegraEMV.getRspCdError() + "\r\n" + cpIntegraEMV.getRspFriendlyResponse();
                result.voucher = "";
                result.status = "error";
                result.parametros = response;
                break;
              case "denied":
                response = cpIntegraEMV.getRspDsResponse() + " NOperacion = " + cpIntegraEMV.getRspOperationNumber() +
                "\r\n CDResp = " + cpIntegraEMV.getRspCdResponse() +
                " Ref = " + cpIntegraEMV.getTx_Reference() + " " + cpIntegraEMV.getRspFriendlyResponse();
                result.voucher = "";
                result.status = "denied";
                result.parametros = response;
                break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      Response.Headers.Add("Access-Control-Allow-Origin", "*");
      return result;
    }

  }
}
