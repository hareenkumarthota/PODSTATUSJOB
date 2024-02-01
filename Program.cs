using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Xml;
using System.Net;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace PODSTATUSJOB
{
    public class Program
    {
        public static string FolderName = DateTime.Now.ToString("ddMMMyyyyHHmmssfff");
        public static string LogPath = "";

        public static void Main(string[] args)
        {
            string connectionstring = "";
            SqlConnection con = null;
            string ERPUpdateFlag = "false";
            string ERPEngingePath = "";
            string ERPNAME = "";
            string LabelPath = "";
            string CarrierBackupPath = "";
            string strTrackQuery = "";

            // addquery("Test :");


            try
            {
                //string strWebConn = ConfigurationManager.ConnectionStrings["xCarrierConnectionString"].ConnectionString;

                string strFilepath = System.IO.File.ReadAllText(System.Windows.Forms.Application.StartupPath + "\\xCarrier_ConnectionString.txt");
                string[] strArrayFilePath = strFilepath.Split('\n');
                if (strArrayFilePath.Length > 0)
                {
                    connectionstring = strArrayFilePath[0].ToString().Trim();
                    //connectionstring = strWebConn;
                    //addquery("1 :" + connectionstring);
                }

            }
            catch (Exception)
            { }

            try
            {
                string strFilepath = System.IO.File.ReadAllText(System.Windows.Forms.Application.StartupPath + "\\Configuration.txt");
                string[] strArrayFilePath = strFilepath.Split('\n');
                if (strArrayFilePath.Length > 0)
                {
                    LabelPath = strArrayFilePath[0].ToString().Trim();
                    ERPUpdateFlag = strArrayFilePath[1].ToString().Trim();
                    ERPEngingePath = strArrayFilePath[2].ToString().Trim();
                    ERPNAME = strArrayFilePath[3].ToString().Trim();
                    CarrierBackupPath = strArrayFilePath[4].ToString().Trim();

                }

            }
            catch (Exception ex)
            {

            }
            try
            {
                string strSQLQuerys = System.IO.File.ReadAllText(System.Windows.Forms.Application.StartupPath + "\\SQLQuerys.txt");
                if (strSQLQuerys != "")
                {
                    strTrackQuery = strSQLQuerys;
                }
            }
            catch (Exception ex)
            {

            }
            try
            {

                Directory.CreateDirectory(CarrierBackupPath + "\\" + FolderName);
                LogPath = CarrierBackupPath + "\\" + FolderName;

            }
            catch (Exception ex)
            {

            }
            con = new SqlConnection(connectionstring);

            SqlDataAdapter sqldaCarrier = new SqlDataAdapter("SELECT [ID],[CARRIERID],[CARRIER],[CARRIER_DESCRIPTION],[ACCOUNT_NUMBER],[METER_NUMBER],[LICENSE_NUMBER],[USER_ID],[PASSWORD],[PASSPHARASE],[WEBPASSWORD],[DISCOUNT],[FUELSURCHAGE],[SPECIALFEE],[ADDTIONALFEE],[URL_CONFIRM],[URL_ACCEPT],[URLVOID],[SHIPPINGKEY],[LTLRATE],[CSP_USERID],[CSP_PASSWORD],[PLANT_ID],[COMPANY_ID],[DEFAULT_ACCOUNTNUMBER],[ACTIVE_CARRIER],[CREATED_BY],[UPDATED_BY],[CREATED_ON],[UPDATED_ON],[LABEL_TYPE],[PAPERLESS_CI],[ENVIRONMENT_TYPE],[DEFAULT_CARRIER],[Global],[CARRIER_PREFIX],[SCAC_CODE],[CARRIER_TYPE],[HUB_ID],[HANDLING_CHARGES],[CARRIER_INTEGRATION],[HANDLING_CHARGE_PERCENTAGE],[PLANT_NAME],[ACCESS_KEY],[TRACKING_TYPE],[TRACKING_FROM],[TRACKING_TO],[DG_AIR],[DG_GROUND],[CLASS],[NMFC],[PACKING],[CLOSE_TIME],[BOL_URL],[RATE_URL],[PICKUP_URL],[READY_TIME],[PICKUP_TIME] FROM [XCARRIER_CARRIER]", con);
            DataSet dsCarrier = new DataSet();
            sqldaCarrier.Fill(dsCarrier);

            addquery("2 :" + connectionstring);


            SqlDataAdapter sqldaCountryCode = new SqlDataAdapter("select COUNTRY,COUNTRY_CODE  from COUNTRYCODES", con);
            DataSet dsCountryCode = new DataSet();
            sqldaCountryCode.Fill(dsCountryCode);

            addquery("2 Query :" + strTrackQuery);

            //string strTrackQuery = "select xcarrier_shipments.DELIVERY_NUM ,xcarrier_shipments.FEEDERSYSTEM_NAME ,xcarrier_shipments.COMPANY_ID,xcarrier_shipments.ship_date,xcarrier_shipments.shipfrom_company,xcarrier_shipments.shipping_num,xcarrier_shipments.carrier_description as carrier,xcarrier_shipments.plant_id,xcarrier_shipments.service_level_name,xcarrier_shipments.account_number, xcarrier_shipments.tracking_number as tracking_num,xcarrier_shipments.tracking_number as MasterTrackingNo from xcarrier_shipments  where xcarrier_shipments.status_code = 'SPD' AND XCARRIER_SHIPMENTS.POD_STATUS <> 'DELIVERED' and (xcarrier_shipments.SHIP_DATE > DATEADD(DAY,-90,GETDATE())) and xcarrier_shipments.CARRIER_DESCRIPTION in('FedEx','UPS','DHL')   and xcarrier_shipments.SHIPPING_NUM in(1003,1002,1006,1008,1316,1202)  order by xcarrier_shipments.SHIP_DATE";
            SqlDataAdapter da = new SqlDataAdapter(strTrackQuery, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                addquery("ds.Tables[0].Rows.Count :" + ds.Tables[0].Rows.Count);
                int iCanadapostCount = 0;

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    addquery("Dataset(1) For loop :" + i);

                    string CustomerID = "";
                    string carrier = "";
                    string AccountNo = "";
                    string TrackingNo = "";
                    string PlantID = "";
                    string Service = "";
                    String strShipFromCountry = "";
                    String strShipDate = "";
                    string ShippingNo = "";
                    string DeliveryNum = "";
                    string FeederSystemName = "";
                    string MastertrackingNo = "";
                    string OrderType = "";

                    string ShipToContact = "";
                    string ShipToAddressline1 = "";
                    string ShipToAddressline2 = "";

                    string ShipTOCity = "";
                    string ShipTOState = "";
                    string ShipTOZipCode = "";
                    string ShipTOCountry = "";



                    CustomerID = ds.Tables[0].Rows[i]["COMPANY_ID"].ToString();
                    carrier = ds.Tables[0].Rows[i]["carrier"].ToString();
                    AccountNo = ds.Tables[0].Rows[i]["account_number"].ToString();
                    TrackingNo = ds.Tables[0].Rows[i]["tracking_num"].ToString();
                    PlantID = ds.Tables[0].Rows[i]["plant_id"].ToString();
                    Service = ds.Tables[0].Rows[i]["service_level_name"].ToString();
                    strShipFromCountry = ds.Tables[0].Rows[i]["shipfrom_company"].ToString();
                    strShipDate = ds.Tables[0].Rows[i]["ship_date"].ToString();
                    ShippingNo = ds.Tables[0].Rows[i]["shipping_num"].ToString();
                    DeliveryNum = ds.Tables[0].Rows[i]["DELIVERY_NUM"].ToString();
                    FeederSystemName = ds.Tables[0].Rows[i]["FEEDERSYSTEM_NAME"].ToString();
                    MastertrackingNo = ds.Tables[0].Rows[i]["MasterTrackingNo"].ToString();
                    OrderType = ds.Tables[0].Rows[i]["ORDER_TYPE"].ToString();

                    ShipToContact = ds.Tables[0].Rows[i]["SHIPTO_CONTACT"].ToString();
                    ShipToAddressline1 = ds.Tables[0].Rows[i]["SHIPTO_ADDRESSLINE1"].ToString();
                    ShipToAddressline2 = ds.Tables[0].Rows[i]["SHIPTO_ADDRESSLINE2"].ToString();

                    ShipTOCity = ds.Tables[0].Rows[i]["SHIPTO_CITY"].ToString();
                    ShipTOState = ds.Tables[0].Rows[i]["SHIPTO_STATE"].ToString();
                    ShipTOZipCode = ds.Tables[0].Rows[i]["SHIPTO_ZIPCODE"].ToString();
                    ShipTOCountry = ds.Tables[0].Rows[i]["SHIPTO_COUNTRY"].ToString();


                    addquery("DataSet(1) For loop carrier:" + carrier + ":: Tracking No :" + TrackingNo + "::ShippingNO : " + ShippingNo);

                    switch (carrier.ToUpper())
                    {
                        //    UPSPod_Status(AccountNo, TrackingNo, PlantID, Service, connectionstring, strShipFromCountry, strShipDate, ShippingNo, ERPEngingePath, 
                        //ERPUpdateFlag, LabelPath, dsCarrier, con, CustomerID, carrier, ERPNAME, DeliveryNum, FeederSystemName, MastertrackingNo);

                        case "UPS":
                            object[] UPSparam_obj = new object[27];
                            UPSparam_obj[0] = AccountNo;
                            UPSparam_obj[1] = TrackingNo;
                            UPSparam_obj[2] = PlantID;
                            UPSparam_obj[3] = Service;
                            UPSparam_obj[4] = connectionstring;
                            UPSparam_obj[5] = strShipFromCountry;
                            UPSparam_obj[6] = strShipDate;
                            UPSparam_obj[7] = ShippingNo;
                            UPSparam_obj[8] = ERPEngingePath;
                            UPSparam_obj[9] = ERPUpdateFlag;
                            UPSparam_obj[10] = LabelPath;
                            UPSparam_obj[11] = dsCarrier;
                            UPSparam_obj[12] = con;
                            UPSparam_obj[13] = CustomerID;
                            UPSparam_obj[14] = carrier;
                            UPSparam_obj[15] = ERPNAME;
                            UPSparam_obj[16] = DeliveryNum;
                            UPSparam_obj[17] = FeederSystemName;
                            UPSparam_obj[18] = MastertrackingNo;
                            UPSparam_obj[19] = OrderType;

                            UPSparam_obj[20] = ShipToContact;
                            UPSparam_obj[21] = ShipToAddressline1;
                            UPSparam_obj[22] = ShipToAddressline2;

                            UPSparam_obj[23] = ShipTOCity;
                            UPSparam_obj[24] = ShipTOState;
                            UPSparam_obj[25] = ShipTOZipCode;
                            UPSparam_obj[26] = ShipTOCountry;


                            Thread UPSthread1 = new Thread(UPSPod_Status);
                            UPSthread1.Name = "UPS Thread ::" + TrackingNo;
                            addquery("UPS Thread name: " + UPSthread1.Name.ToString());
                            UPSthread1.Start(UPSparam_obj);

                            while (UPSthread1.IsAlive)
                            {
                                try
                                {
                                    UPSthread1.Join();
                                }
                                catch (Exception ex)
                                {

                                }

                            }
                            UPSthread1.Abort();
                            System.Threading.Thread.Sleep(50);

                            break;


                        case "FEDEX":

                            object[] Fedexparam_obj = new object[27];
                            Fedexparam_obj[0] = AccountNo;
                            Fedexparam_obj[1] = TrackingNo;
                            Fedexparam_obj[2] = PlantID;
                            Fedexparam_obj[3] = Service;
                            Fedexparam_obj[4] = connectionstring;
                            Fedexparam_obj[5] = strShipFromCountry;
                            Fedexparam_obj[6] = strShipDate;
                            Fedexparam_obj[7] = ShippingNo;
                            Fedexparam_obj[8] = ERPEngingePath;
                            Fedexparam_obj[9] = ERPUpdateFlag;
                            Fedexparam_obj[10] = LabelPath;
                            Fedexparam_obj[11] = dsCarrier;
                            Fedexparam_obj[12] = con;
                            Fedexparam_obj[13] = CustomerID;
                            Fedexparam_obj[14] = carrier;
                            Fedexparam_obj[15] = ERPNAME;
                            Fedexparam_obj[16] = DeliveryNum;
                            Fedexparam_obj[17] = FeederSystemName;
                            Fedexparam_obj[18] = MastertrackingNo;
                            Fedexparam_obj[19] = OrderType;

                            Fedexparam_obj[20] = ShipToContact;
                            Fedexparam_obj[21] = ShipToAddressline1;
                            Fedexparam_obj[22] = ShipToAddressline2;

                            Fedexparam_obj[23] = ShipTOCity;
                            Fedexparam_obj[24] = ShipTOState;
                            Fedexparam_obj[25] = ShipTOZipCode;
                            Fedexparam_obj[26] = ShipTOCountry;


                            //XS.SHIPTO_CONTACT,XS.SHIPTO_ADDRESSLINE1,XS.SHIPTO_ADDRESSLINE2,

                            Thread Fedexthread1 = new Thread(FEDEX_Status);
                            Fedexthread1.Name = "Fedex Thread ::" + TrackingNo;
                            addquery("FedexThread name: " + Fedexthread1.Name.ToString());
                            Fedexthread1.Start(Fedexparam_obj);

                            while (Fedexthread1.IsAlive)
                            {
                                try
                                {
                                    Fedexthread1.Join();
                                }
                                catch (Exception ex)
                                {

                                }

                            }
                            Fedexthread1.Abort();
                            System.Threading.Thread.Sleep(50);

                            break;

                        case "FEDEXRESTAPI":

                            object[] FEDEXRESTAPIparam_obj = new object[27];
                            FEDEXRESTAPIparam_obj[0] = AccountNo;
                            FEDEXRESTAPIparam_obj[1] = TrackingNo;
                            FEDEXRESTAPIparam_obj[2] = PlantID;
                            FEDEXRESTAPIparam_obj[3] = Service;
                            FEDEXRESTAPIparam_obj[4] = connectionstring;
                            FEDEXRESTAPIparam_obj[5] = strShipFromCountry;
                            FEDEXRESTAPIparam_obj[6] = strShipDate;
                            FEDEXRESTAPIparam_obj[7] = ShippingNo;
                            FEDEXRESTAPIparam_obj[8] = ERPEngingePath;
                            FEDEXRESTAPIparam_obj[9] = ERPUpdateFlag;
                            FEDEXRESTAPIparam_obj[10] = LabelPath;
                            FEDEXRESTAPIparam_obj[11] = dsCarrier;
                            FEDEXRESTAPIparam_obj[12] = con;
                            FEDEXRESTAPIparam_obj[13] = CustomerID;
                            FEDEXRESTAPIparam_obj[14] = carrier;
                            FEDEXRESTAPIparam_obj[15] = ERPNAME;
                            FEDEXRESTAPIparam_obj[16] = DeliveryNum;
                            FEDEXRESTAPIparam_obj[17] = FeederSystemName;
                            FEDEXRESTAPIparam_obj[18] = MastertrackingNo;
                            FEDEXRESTAPIparam_obj[19] = OrderType;

                            FEDEXRESTAPIparam_obj[20] = ShipToContact;
                            FEDEXRESTAPIparam_obj[21] = ShipToAddressline1;
                            FEDEXRESTAPIparam_obj[22] = ShipToAddressline2;

                            FEDEXRESTAPIparam_obj[23] = ShipTOCity;
                            FEDEXRESTAPIparam_obj[24] = ShipTOState;
                            FEDEXRESTAPIparam_obj[25] = ShipTOZipCode;
                            FEDEXRESTAPIparam_obj[26] = ShipTOCountry;


                            //XS.SHIPTO_CONTACT,XS.SHIPTO_ADDRESSLINE1,XS.SHIPTO_ADDRESSLINE2,

                            Thread FedexRESTthread1 = new Thread(FEDEX_Status);
                            FedexRESTthread1.Name = "Fedex Thread ::" + TrackingNo;
                            addquery("FedexThread name: " + FedexRESTthread1.Name.ToString());
                            FedexRESTthread1.Start(FEDEXRESTAPIparam_obj);

                            while (FedexRESTthread1.IsAlive)
                            {
                                try
                                {
                                    FedexRESTthread1.Join();
                                }
                                catch (Exception ex)
                                {

                                }

                            }
                            FedexRESTthread1.Abort();
                            System.Threading.Thread.Sleep(50);

                            break;

                        case "COLISSIMO":

                            object[] CPLISSIMOparam_obj = new object[21];
                            CPLISSIMOparam_obj[0] = AccountNo;
                            CPLISSIMOparam_obj[1] = TrackingNo;
                            CPLISSIMOparam_obj[2] = PlantID;
                            CPLISSIMOparam_obj[3] = Service;
                            CPLISSIMOparam_obj[4] = connectionstring;
                            CPLISSIMOparam_obj[5] = strShipFromCountry;
                            CPLISSIMOparam_obj[6] = strShipDate;
                            CPLISSIMOparam_obj[7] = ShippingNo;
                            CPLISSIMOparam_obj[8] = ERPEngingePath;
                            CPLISSIMOparam_obj[9] = ERPUpdateFlag;
                            CPLISSIMOparam_obj[10] = LabelPath;
                            CPLISSIMOparam_obj[11] = dsCarrier;
                            CPLISSIMOparam_obj[12] = con;
                            CPLISSIMOparam_obj[13] = CustomerID;
                            CPLISSIMOparam_obj[14] = carrier;
                            CPLISSIMOparam_obj[15] = ERPNAME;
                            CPLISSIMOparam_obj[16] = DeliveryNum;
                            CPLISSIMOparam_obj[17] = FeederSystemName;
                            CPLISSIMOparam_obj[18] = MastertrackingNo;
                            CPLISSIMOparam_obj[19] = dsCountryCode;
                            CPLISSIMOparam_obj[20] = OrderType;

                            Thread COLISSIMOthread1 = new Thread(COLLISIMO_Status);
                            COLISSIMOthread1.Name = "COLLISIMO Thread ::" + TrackingNo;
                            addquery("COLLISIMO name: " + COLISSIMOthread1.Name.ToString());
                            COLISSIMOthread1.Start(CPLISSIMOparam_obj);

                            while (COLISSIMOthread1.IsAlive)
                            {
                                try
                                {
                                    COLISSIMOthread1.Join();
                                }
                                catch (Exception ex)
                                {

                                }

                            }
                            COLISSIMOthread1.Abort();
                            System.Threading.Thread.Sleep(50);

                            break;
                        case "PUROLATORESHIP":

                            object[] PUROLATORESHIPparam_obj = new object[20];
                            PUROLATORESHIPparam_obj[0] = AccountNo;
                            PUROLATORESHIPparam_obj[1] = TrackingNo;
                            PUROLATORESHIPparam_obj[2] = PlantID;
                            PUROLATORESHIPparam_obj[3] = Service;
                            PUROLATORESHIPparam_obj[4] = connectionstring;
                            PUROLATORESHIPparam_obj[5] = strShipFromCountry;
                            PUROLATORESHIPparam_obj[6] = strShipDate;
                            PUROLATORESHIPparam_obj[7] = ShippingNo;
                            PUROLATORESHIPparam_obj[8] = ERPEngingePath;
                            PUROLATORESHIPparam_obj[9] = ERPUpdateFlag;
                            PUROLATORESHIPparam_obj[10] = LabelPath;
                            PUROLATORESHIPparam_obj[11] = dsCarrier;
                            PUROLATORESHIPparam_obj[12] = con;
                            PUROLATORESHIPparam_obj[13] = CustomerID;
                            PUROLATORESHIPparam_obj[14] = carrier;
                            PUROLATORESHIPparam_obj[15] = ERPNAME;
                            PUROLATORESHIPparam_obj[16] = DeliveryNum;
                            PUROLATORESHIPparam_obj[17] = FeederSystemName;
                            PUROLATORESHIPparam_obj[18] = MastertrackingNo;
                            PUROLATORESHIPparam_obj[19] = OrderType;

                            Thread PUROLATORESHIPthread1 = new Thread(PUROLATORESHIP_Status);
                            PUROLATORESHIPthread1.Name = "PUROLATORESHIPthread1 Thread ::" + TrackingNo;
                            addquery("PUROLATORESHIP name: " + PUROLATORESHIPthread1.Name.ToString());
                            PUROLATORESHIPthread1.Start(PUROLATORESHIPparam_obj);

                            while (PUROLATORESHIPthread1.IsAlive)
                            {
                                try
                                {
                                    PUROLATORESHIPthread1.Join();
                                }
                                catch (Exception ex)
                                {

                                }

                            }
                            PUROLATORESHIPthread1.Abort();
                            break;
                        case "CANADAPOST":

                            object[] CANADAPOSTparam_obj = new object[20];
                            CANADAPOSTparam_obj[0] = AccountNo;
                            CANADAPOSTparam_obj[1] = TrackingNo;
                            CANADAPOSTparam_obj[2] = PlantID;
                            CANADAPOSTparam_obj[3] = Service;
                            CANADAPOSTparam_obj[4] = connectionstring;
                            CANADAPOSTparam_obj[5] = strShipFromCountry;
                            CANADAPOSTparam_obj[6] = strShipDate;
                            CANADAPOSTparam_obj[7] = ShippingNo;
                            CANADAPOSTparam_obj[8] = ERPEngingePath;
                            CANADAPOSTparam_obj[9] = ERPUpdateFlag;
                            CANADAPOSTparam_obj[10] = LabelPath;
                            CANADAPOSTparam_obj[11] = dsCarrier;
                            CANADAPOSTparam_obj[12] = con;
                            CANADAPOSTparam_obj[13] = CustomerID;
                            CANADAPOSTparam_obj[14] = carrier;
                            CANADAPOSTparam_obj[15] = ERPNAME;
                            CANADAPOSTparam_obj[16] = DeliveryNum;
                            CANADAPOSTparam_obj[17] = FeederSystemName;
                            CANADAPOSTparam_obj[18] = MastertrackingNo;
                            CANADAPOSTparam_obj[19] = OrderType;

                            Thread CANADAPOSTthread1 = new Thread(CANADAPOST_Status);
                            CANADAPOSTthread1.Name = "CANADAPOST Thread ::" + TrackingNo;
                            addquery("CANADAPOST name: " + CANADAPOSTthread1.Name.ToString());
                            CANADAPOSTthread1.Start(CANADAPOSTparam_obj);

                            while (CANADAPOSTthread1.IsAlive)
                            {
                                try
                                {
                                    CANADAPOSTthread1.Join();
                                }
                                catch (Exception ex)
                                {

                                }

                            }
                            CANADAPOSTthread1.Abort();

                            if (iCanadapostCount % 20 == 0)
                            {
                                System.Threading.Thread.Sleep(60000);
                            }
                            else
                            {
                                System.Threading.Thread.Sleep(50);
                            }
                            iCanadapostCount = iCanadapostCount + 1;

                            break;


                        default:
                            addquery("Default statemenr" + i + "No records Update");
                            break;
                    }

                    System.Threading.Thread.Sleep(100);
                }
            }
            if (ds.Tables.Count > 1)
            {
                int iODFLCount = 0;
                if (ds.Tables[1].Rows.Count > 0)
                {

                    addquery("ds.Tables[1].Rows.Count ::" + ds.Tables[1].Rows.Count);

                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        addquery("Dataset(2) For loop :" + i);
                        string CustomerID = "";
                        string carrier = "";
                        string AccountNo = "";
                        string TrackingNo = "";
                        string PlantID = "";
                        string Service = "";
                        String strShipFromCountry = "";
                        String strShipDate = "";
                        string ShippingNo = "";
                        string DeliveryNum = "";
                        string FeederSystemName = "";
                        string MastertrackingNo = "";
                        string strShipToCountry = "";
                        string strShipfrom_company = "";
                        string strOrderType = "";

                        CustomerID = ds.Tables[1].Rows[i]["COMPANY_ID"].ToString();
                        carrier = ds.Tables[1].Rows[i]["carrier"].ToString();
                        AccountNo = ds.Tables[1].Rows[i]["account_number"].ToString();
                        TrackingNo = ds.Tables[1].Rows[i]["tracking_num"].ToString();
                        PlantID = ds.Tables[1].Rows[i]["plant_id"].ToString();
                        Service = ds.Tables[1].Rows[i]["service_level_name"].ToString();
                        strShipfrom_company = ds.Tables[1].Rows[i]["shipfrom_company"].ToString();
                        strShipDate = ds.Tables[1].Rows[i]["ship_date"].ToString();
                        ShippingNo = ds.Tables[1].Rows[i]["shipping_num"].ToString();
                        DeliveryNum = ds.Tables[1].Rows[i]["DELIVERY_NUM"].ToString();
                        FeederSystemName = ds.Tables[1].Rows[i]["FEEDERSYSTEM_NAME"].ToString();
                        MastertrackingNo = ds.Tables[1].Rows[i]["MasterTrackingNo"].ToString();
                        strShipFromCountry = ds.Tables[1].Rows[i]["SHIPFROM_COUNTRY"].ToString();
                        strShipToCountry = ds.Tables[1].Rows[i]["SHIPTO_COUNTRY"].ToString();
                        strOrderType = ds.Tables[1].Rows[i]["ORDER_TYPE"].ToString();

                        addquery("DataSet(2) For loop carrier:" + carrier + ":: Tracking No :" + TrackingNo + "::ShippingNO : " + ShippingNo);

                        switch (carrier.ToUpper())
                        {

                            case "UPS FREIGHT":

                                object[] UPSFreightparam_obj = new object[20];
                                UPSFreightparam_obj[0] = AccountNo;
                                UPSFreightparam_obj[1] = TrackingNo;
                                UPSFreightparam_obj[2] = PlantID;
                                UPSFreightparam_obj[3] = Service;
                                UPSFreightparam_obj[4] = connectionstring;
                                UPSFreightparam_obj[5] = strShipFromCountry;
                                UPSFreightparam_obj[6] = strShipDate;
                                UPSFreightparam_obj[7] = ShippingNo;
                                UPSFreightparam_obj[8] = ERPEngingePath;
                                UPSFreightparam_obj[9] = ERPUpdateFlag;
                                UPSFreightparam_obj[10] = LabelPath;
                                UPSFreightparam_obj[11] = dsCarrier;
                                UPSFreightparam_obj[12] = con;
                                UPSFreightparam_obj[13] = CustomerID;
                                UPSFreightparam_obj[14] = carrier;
                                UPSFreightparam_obj[15] = ERPNAME;
                                UPSFreightparam_obj[16] = DeliveryNum;
                                UPSFreightparam_obj[17] = FeederSystemName;
                                UPSFreightparam_obj[18] = MastertrackingNo;
                                UPSFreightparam_obj[19] = strOrderType;

                                Thread UPSFreighthread1 = new Thread(UPS_FREIGHT_Status);
                                UPSFreighthread1.Name = "UPS_FREIGHT Thread ::" + TrackingNo;
                                addquery("UPS_FREIGHT Thread name: " + UPSFreighthread1.Name.ToString());
                                UPSFreighthread1.Start(UPSFreightparam_obj);

                                while (UPSFreighthread1.IsAlive)
                                {
                                    try
                                    {
                                        UPSFreighthread1.Join();
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                }
                                UPSFreighthread1.Abort();
                                System.Threading.Thread.Sleep(50);

                                break;
                            case "ODFL":

                                object[] ODFLparam_obj = new object[20];
                                ODFLparam_obj[0] = AccountNo;
                                ODFLparam_obj[1] = TrackingNo;
                                ODFLparam_obj[2] = PlantID;
                                ODFLparam_obj[3] = Service;
                                ODFLparam_obj[4] = connectionstring;
                                ODFLparam_obj[5] = strShipFromCountry;
                                ODFLparam_obj[6] = strShipDate;
                                ODFLparam_obj[7] = ShippingNo;
                                ODFLparam_obj[8] = ERPEngingePath;
                                ODFLparam_obj[9] = ERPUpdateFlag;
                                ODFLparam_obj[10] = LabelPath;
                                ODFLparam_obj[11] = dsCarrier;
                                ODFLparam_obj[12] = con;
                                ODFLparam_obj[13] = CustomerID;
                                ODFLparam_obj[14] = carrier;
                                ODFLparam_obj[15] = ERPNAME;
                                ODFLparam_obj[16] = DeliveryNum;
                                ODFLparam_obj[17] = FeederSystemName;
                                ODFLparam_obj[18] = MastertrackingNo;
                                //ODFLparam_obj[19] = OrderType;

                                Thread ODFLthread1 = new Thread(ODFL_Status);
                                ODFLthread1.Name = "ODFL Thread ::" + TrackingNo;
                                addquery("ODFL name: " + ODFLthread1.Name.ToString());
                                ODFLthread1.Start(ODFLparam_obj);

                                while (ODFLthread1.IsAlive)
                                {
                                    try
                                    {
                                        ODFLthread1.Join();
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                }
                                ODFLthread1.Abort();

                                if (iODFLCount % 20 == 0)
                                {
                                    System.Threading.Thread.Sleep(60000);
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(50);
                                }
                                iODFLCount = iODFLCount + 1;

                                break;

                            case "FEDEX FREIGHT":

                                object[] FEDEXFreightparam_obj = new object[20];
                                FEDEXFreightparam_obj[0] = AccountNo;
                                FEDEXFreightparam_obj[1] = TrackingNo;
                                FEDEXFreightparam_obj[2] = PlantID;
                                FEDEXFreightparam_obj[3] = Service;
                                FEDEXFreightparam_obj[4] = connectionstring;
                                FEDEXFreightparam_obj[5] = strShipFromCountry;
                                FEDEXFreightparam_obj[6] = strShipDate;
                                FEDEXFreightparam_obj[7] = ShippingNo;
                                FEDEXFreightparam_obj[8] = ERPEngingePath;
                                FEDEXFreightparam_obj[9] = ERPUpdateFlag;
                                FEDEXFreightparam_obj[10] = LabelPath;
                                FEDEXFreightparam_obj[11] = dsCarrier;
                                FEDEXFreightparam_obj[12] = con;
                                FEDEXFreightparam_obj[13] = CustomerID;
                                FEDEXFreightparam_obj[14] = carrier;
                                FEDEXFreightparam_obj[15] = ERPNAME;
                                FEDEXFreightparam_obj[16] = DeliveryNum;
                                FEDEXFreightparam_obj[17] = FeederSystemName;
                                FEDEXFreightparam_obj[18] = MastertrackingNo;
                                FEDEXFreightparam_obj[19] = strOrderType;

                                Thread FEDEXFreighthread1 = new Thread(FEDEX_FREIGHT_Status);
                                FEDEXFreighthread1.Name = "FEDEX_FREIGHT Thread ::" + TrackingNo;
                                addquery("FEDEX_FREIGHT Thread name: " + FEDEXFreighthread1.Name.ToString());
                                FEDEXFreighthread1.Start(FEDEXFreightparam_obj);

                                while (FEDEXFreighthread1.IsAlive)
                                {
                                    try
                                    {
                                        FEDEXFreighthread1.Join();
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                FEDEXFreighthread1.Abort();
                                System.Threading.Thread.Sleep(50);

                                break;
                            case "ESTES":

                                object[] ESTESparam_obj = new object[20];
                                ESTESparam_obj[0] = AccountNo;
                                ESTESparam_obj[1] = TrackingNo;
                                ESTESparam_obj[2] = PlantID;
                                ESTESparam_obj[3] = Service;
                                ESTESparam_obj[4] = connectionstring;
                                ESTESparam_obj[5] = strShipFromCountry;
                                ESTESparam_obj[6] = strShipDate;
                                ESTESparam_obj[7] = ShippingNo;
                                ESTESparam_obj[8] = ERPEngingePath;
                                ESTESparam_obj[9] = ERPUpdateFlag;
                                ESTESparam_obj[10] = LabelPath;
                                ESTESparam_obj[11] = dsCarrier;
                                ESTESparam_obj[12] = con;
                                ESTESparam_obj[13] = CustomerID;
                                ESTESparam_obj[14] = carrier;
                                ESTESparam_obj[15] = ERPNAME;
                                ESTESparam_obj[16] = DeliveryNum;
                                ESTESparam_obj[17] = FeederSystemName;
                                ESTESparam_obj[18] = MastertrackingNo;
                                ESTESparam_obj[19] = strOrderType;

                                Thread ESTEShread1 = new Thread(ESTES_Status);
                                ESTEShread1.Name = "ESTES Thread ::" + TrackingNo;
                                addquery("ESTES Thread name: " + ESTEShread1.Name.ToString());
                                ESTEShread1.Start(ESTESparam_obj);

                                while (ESTEShread1.IsAlive)
                                {
                                    try
                                    {
                                        ESTEShread1.Join();
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                ESTEShread1.Abort();
                                System.Threading.Thread.Sleep(50);

                                break;

                            case "SAIA":

                                object[] SAIAparam_obj = new object[20];
                                SAIAparam_obj[0] = AccountNo;
                                SAIAparam_obj[1] = TrackingNo;
                                SAIAparam_obj[2] = PlantID;
                                SAIAparam_obj[3] = Service;
                                SAIAparam_obj[4] = connectionstring;
                                SAIAparam_obj[5] = strShipFromCountry;
                                SAIAparam_obj[6] = strShipDate;
                                SAIAparam_obj[7] = ShippingNo;
                                SAIAparam_obj[8] = ERPEngingePath;
                                SAIAparam_obj[9] = ERPUpdateFlag;
                                SAIAparam_obj[10] = LabelPath;
                                SAIAparam_obj[11] = dsCarrier;
                                SAIAparam_obj[12] = con;
                                SAIAparam_obj[13] = CustomerID;
                                SAIAparam_obj[14] = carrier;
                                SAIAparam_obj[15] = ERPNAME;
                                SAIAparam_obj[16] = DeliveryNum;
                                SAIAparam_obj[17] = FeederSystemName;
                                SAIAparam_obj[18] = MastertrackingNo;
                                SAIAparam_obj[19] = strOrderType;

                                Thread SAIAhread1 = new Thread(SAIA_Status);
                                SAIAhread1.Name = "SAIA Thread ::" + TrackingNo;
                                addquery("SAIA Thread name: " + SAIAhread1.Name.ToString());
                                SAIAhread1.Start(SAIAparam_obj);

                                while (SAIAhread1.IsAlive)
                                {
                                    try
                                    {
                                        SAIAhread1.Join();
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                SAIAhread1.Abort();
                                System.Threading.Thread.Sleep(50);

                                break;
                            case "SEFL":

                                object[] SEFLparam_obj = new object[20];
                                SEFLparam_obj[0] = AccountNo;
                                SEFLparam_obj[1] = TrackingNo;
                                SEFLparam_obj[2] = PlantID;
                                SEFLparam_obj[3] = Service;
                                SEFLparam_obj[4] = connectionstring;
                                SEFLparam_obj[5] = strShipFromCountry;
                                SEFLparam_obj[6] = strShipDate;
                                SEFLparam_obj[7] = ShippingNo;
                                SEFLparam_obj[8] = ERPEngingePath;
                                SEFLparam_obj[9] = ERPUpdateFlag;
                                SEFLparam_obj[10] = LabelPath;
                                SEFLparam_obj[11] = dsCarrier;
                                SEFLparam_obj[12] = con;
                                SEFLparam_obj[13] = CustomerID;
                                SEFLparam_obj[14] = carrier;
                                SEFLparam_obj[15] = ERPNAME;
                                SEFLparam_obj[16] = DeliveryNum;
                                SEFLparam_obj[17] = FeederSystemName;
                                SEFLparam_obj[18] = MastertrackingNo;
                                SEFLparam_obj[19] = strOrderType;

                                Thread SEFLhread1 = new Thread(SEFL_Status);
                                SEFLhread1.Name = "SEFL Thread ::" + TrackingNo;
                                addquery("SEFL Thread name: " + SEFLhread1.Name.ToString());
                                SEFLhread1.Start(SEFLparam_obj);

                                while (SEFLhread1.IsAlive)
                                {
                                    try
                                    {
                                        SEFLhread1.Join();
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                SEFLhread1.Abort();
                                System.Threading.Thread.Sleep(50);

                                break;

                            case "RL":

                                object[] RLparam_obj = new object[20];
                                RLparam_obj[0] = AccountNo;
                                RLparam_obj[1] = TrackingNo;
                                RLparam_obj[2] = PlantID;
                                RLparam_obj[3] = Service;
                                RLparam_obj[4] = connectionstring;
                                RLparam_obj[5] = strShipFromCountry;
                                RLparam_obj[6] = strShipDate;
                                RLparam_obj[7] = ShippingNo;
                                RLparam_obj[8] = ERPEngingePath;
                                RLparam_obj[9] = ERPUpdateFlag;
                                RLparam_obj[10] = LabelPath;
                                RLparam_obj[11] = dsCarrier;
                                RLparam_obj[12] = con;
                                RLparam_obj[13] = CustomerID;
                                RLparam_obj[14] = carrier;
                                RLparam_obj[15] = ERPNAME;
                                RLparam_obj[16] = DeliveryNum;
                                RLparam_obj[17] = FeederSystemName;
                                RLparam_obj[18] = MastertrackingNo;
                                RLparam_obj[19] = strOrderType;

                                Thread RLThread1 = new Thread(RL_Status);
                                RLThread1.Name = "RL LTL Thread ::" + TrackingNo;
                                addquery("RL LTL Thread name: " + RLThread1.Name.ToString());
                                RLThread1.Start(RLparam_obj);

                                while (RLThread1.IsAlive)
                                {
                                    try
                                    {
                                        RLThread1.Join();
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                RLThread1.Abort();
                                System.Threading.Thread.Sleep(50);

                                break;

                            case "DAYTONFREIGHT":

                                object[] DAYTONFREIGHTparam_obj = new object[20];
                                DAYTONFREIGHTparam_obj[0] = AccountNo;
                                DAYTONFREIGHTparam_obj[1] = TrackingNo;
                                DAYTONFREIGHTparam_obj[2] = PlantID;
                                DAYTONFREIGHTparam_obj[3] = Service;
                                DAYTONFREIGHTparam_obj[4] = connectionstring;
                                DAYTONFREIGHTparam_obj[5] = strShipFromCountry;
                                DAYTONFREIGHTparam_obj[6] = strShipDate;
                                DAYTONFREIGHTparam_obj[7] = ShippingNo;
                                DAYTONFREIGHTparam_obj[8] = ERPEngingePath;
                                DAYTONFREIGHTparam_obj[9] = ERPUpdateFlag;
                                DAYTONFREIGHTparam_obj[10] = LabelPath;
                                DAYTONFREIGHTparam_obj[11] = dsCarrier;
                                DAYTONFREIGHTparam_obj[12] = con;
                                DAYTONFREIGHTparam_obj[13] = CustomerID;
                                DAYTONFREIGHTparam_obj[14] = carrier;
                                DAYTONFREIGHTparam_obj[15] = ERPNAME;
                                DAYTONFREIGHTparam_obj[16] = DeliveryNum;
                                DAYTONFREIGHTparam_obj[17] = FeederSystemName;
                                DAYTONFREIGHTparam_obj[18] = MastertrackingNo;
                                DAYTONFREIGHTparam_obj[19] = strOrderType;

                                Thread DAYTONFREIGHTThread1 = new Thread(DAYTONFREIGHT_Status);
                                DAYTONFREIGHTThread1.Name = "DAYTONFREIGHT LTL Thread ::" + TrackingNo;
                                addquery("DAYTONFREIGHT LTL Thread name: " + DAYTONFREIGHTThread1.Name.ToString());
                                DAYTONFREIGHTThread1.Start(DAYTONFREIGHTparam_obj);

                                while (DAYTONFREIGHTThread1.IsAlive)
                                {
                                    try
                                    {
                                        DAYTONFREIGHTThread1.Join();
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                DAYTONFREIGHTThread1.Abort();
                                System.Threading.Thread.Sleep(50);

                                break;

                            case "TNT":

                                object[] TNTparam_obj = new object[20];
                                TNTparam_obj[0] = AccountNo;
                                TNTparam_obj[1] = TrackingNo;
                                TNTparam_obj[2] = PlantID;
                                TNTparam_obj[3] = Service;
                                TNTparam_obj[4] = connectionstring;
                                TNTparam_obj[5] = strShipFromCountry;
                                TNTparam_obj[6] = strShipDate;
                                TNTparam_obj[7] = ShippingNo;
                                TNTparam_obj[8] = ERPEngingePath;
                                TNTparam_obj[9] = ERPUpdateFlag;
                                TNTparam_obj[10] = LabelPath;
                                TNTparam_obj[11] = dsCarrier;
                                TNTparam_obj[12] = con;
                                TNTparam_obj[13] = CustomerID;
                                TNTparam_obj[14] = carrier;
                                TNTparam_obj[15] = ERPNAME;
                                TNTparam_obj[16] = DeliveryNum;
                                TNTparam_obj[17] = FeederSystemName;
                                TNTparam_obj[18] = MastertrackingNo;
                                TNTparam_obj[19] = strOrderType;

                                Thread TNTthread1 = new Thread(TNT_Carrier_Status);
                                TNTthread1.Name = "TNT_Carrier_Status Thread ::" + TrackingNo;
                                addquery("TNT_Carrier_Status Thread name: " + TNTthread1.Name.ToString());
                                TNTthread1.Start(TNTparam_obj);

                                while (TNTthread1.IsAlive)
                                {
                                    try
                                    {
                                        TNTthread1.Join();
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                TNTthread1.Abort();
                                System.Threading.Thread.Sleep(50);

                                break;
                            case "LOVEEXPRESS":

                                object[] LOVEEXPRESSparam_obj = new object[20];
                                LOVEEXPRESSparam_obj[0] = AccountNo;
                                LOVEEXPRESSparam_obj[1] = TrackingNo;
                                LOVEEXPRESSparam_obj[2] = PlantID;
                                LOVEEXPRESSparam_obj[3] = Service;
                                LOVEEXPRESSparam_obj[4] = connectionstring;
                                LOVEEXPRESSparam_obj[5] = strShipFromCountry;
                                LOVEEXPRESSparam_obj[6] = strShipDate;
                                LOVEEXPRESSparam_obj[7] = ShippingNo;
                                LOVEEXPRESSparam_obj[8] = ERPEngingePath;
                                LOVEEXPRESSparam_obj[9] = ERPUpdateFlag;
                                LOVEEXPRESSparam_obj[10] = LabelPath;
                                LOVEEXPRESSparam_obj[11] = dsCarrier;
                                LOVEEXPRESSparam_obj[12] = con;
                                LOVEEXPRESSparam_obj[13] = CustomerID;
                                LOVEEXPRESSparam_obj[14] = carrier;
                                LOVEEXPRESSparam_obj[15] = ERPNAME;
                                LOVEEXPRESSparam_obj[16] = DeliveryNum;
                                LOVEEXPRESSparam_obj[17] = FeederSystemName;
                                LOVEEXPRESSparam_obj[18] = MastertrackingNo;
                                LOVEEXPRESSparam_obj[19] = strOrderType;

                                Thread LOVEEXPRESSthread1 = new Thread(LOVEEXPRESS_Carrier_Status);
                                LOVEEXPRESSthread1.Name = "LOVEEXPRESS_Carrier_Status Thread ::" + TrackingNo;
                                addquery("LOVEEXPRESS_Carrier_Status Thread name: " + LOVEEXPRESSthread1.Name.ToString());
                                LOVEEXPRESSthread1.Start(LOVEEXPRESSparam_obj);

                                while (LOVEEXPRESSthread1.IsAlive)
                                {
                                    try
                                    {
                                        LOVEEXPRESSthread1.Join();
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                LOVEEXPRESSthread1.Abort();
                                System.Threading.Thread.Sleep(50);

                                break;
                            case "DHL":

                                object[] DHLparam_obj = new object[20];
                                DHLparam_obj[0] = AccountNo;
                                DHLparam_obj[1] = TrackingNo;
                                DHLparam_obj[2] = PlantID;
                                DHLparam_obj[3] = Service;
                                DHLparam_obj[4] = connectionstring;
                                DHLparam_obj[5] = strShipFromCountry;
                                DHLparam_obj[6] = strShipDate;
                                DHLparam_obj[7] = ShippingNo;
                                DHLparam_obj[8] = ERPEngingePath;
                                DHLparam_obj[9] = ERPUpdateFlag;
                                DHLparam_obj[10] = LabelPath;
                                DHLparam_obj[11] = dsCarrier;
                                DHLparam_obj[12] = con;
                                DHLparam_obj[13] = CustomerID;
                                DHLparam_obj[14] = carrier;
                                DHLparam_obj[15] = ERPNAME;
                                DHLparam_obj[16] = DeliveryNum;
                                DHLparam_obj[17] = FeederSystemName;
                                DHLparam_obj[18] = MastertrackingNo;
                                DHLparam_obj[19] = strOrderType;

                                Thread DHLhread1 = new Thread(DHL_Status);
                                DHLhread1.Name = "DHL Thread ::" + TrackingNo;
                                addquery("DHL Thread name: " + DHLhread1.Name.ToString());
                                DHLhread1.Start(DHLparam_obj);

                                while (DHLhread1.IsAlive)
                                {
                                    try
                                    {
                                        DHLhread1.Join();
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                }
                                DHLhread1.Abort();
                                System.Threading.Thread.Sleep(50);

                                break;

                            default:
                                addquery("Default statemenr" + i + "No records Update");
                                break;
                        }

                        System.Threading.Thread.Sleep(100);
                    }
                }
            }

            Console.WriteLine("POD-Complete");
            // Console.Read();
        }

        private static String getContryCode(String strCountryCode)
        {
            String strThreeCountryCode = "";
            string Filepath = "C:\\Shipping\\ecs\\Connection1.txt";
            System.IO.StreamReader myFile = new System.IO.StreamReader(Filepath);
            String connectionstring = myFile.ReadToEnd();
            SqlConnection con = new SqlConnection(connectionstring);
            SqlDataAdapter da = new SqlDataAdapter("select countryname from countrycodes1 where countrycode='" + strCountryCode + "'", con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 1)
            {
                strThreeCountryCode = ds.Tables[0].Rows[0]["countryname"].ToString();
            }
            else
            {
                strThreeCountryCode = strCountryCode;
            }

            return strThreeCountryCode;
        }
        //public static void UPSPod_Status123(string AccountNo, string TrackingNo, string PlantID, string Service, string connectionstring, String strShipFromCountry, String strDate, string shippingno, string erpenginepath, string ERPUpdateFlag, string LabelPath, DataSet dsCarrier, SqlConnection con, string CustomerID, string carrier, string ERPName, string DeliveryNUM, string FeederSystemName, string MastertrackingNo, DataSet dsCountryCode)
        //{

        //    string LicenseNo = "";
        //    string UserId = "";
        //    string Password = "";
        //    string _accountno = "";
        //    string custid = "";


        //    string strUpdateQuery = string.Empty;
        //    string strInsertQuery = string.Empty;

        //    XmlDocument MyxmlDocument = new XmlDocument();




        //    DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number
        //                                                                                                                                                                                                         //double itemweight = 0.0;
        //    if (dataRows.Length > 0)
        //    {
        //        LicenseNo = dataRows[0]["LICENSE_NUMBER"].ToString();
        //        UserId = dataRows[0]["User_ID"].ToString();
        //        Password = dataRows[0]["Password"].ToString();
        //        _accountno = dataRows[0]["Account_Number"].ToString();
        //        custid = dataRows[0]["COMPANY_ID"].ToString();

        //    }

        //    //LicenseNo = "FC45B4B21F34AF0C";
        //    //UserId = "kobayashi0wa";
        //    //Password = "Process123";
        //    //_accountno = "6280WA";    
        //    //custid = "9243700001";
        //    string Upsrequest = "";
        //    string status = "";

        //    try
        //    {



        //        Upsrequest = "";
        //        Upsrequest = Upsrequest + "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:v1=\"http://www.ups.com/XMLSchema/XOLTWS/UPSS/v1.0\" xmlns:v2=\"http://www.ups.com/XMLSchema/XOLTWS/Track/v2.0\" xmlns:v11=\"http://www.ups.com/XMLSchema/XOLTWS/Common/v1.0\">" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "<soapenv:Header>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "<v1:UPSSecurity>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "<v1:UsernameToken>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "<v1:Username>" + UserId + "</v1:Username>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "<v1:Password>" + Password + "</v1:Password>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "</v1:UsernameToken>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "<v1:ServiceAccessToken>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "<v1:AccessLicenseNumber>" + LicenseNo + "</v1:AccessLicenseNumber>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "</v1:ServiceAccessToken>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "</v1:UPSSecurity>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "</soapenv:Header>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "<soapenv:Body>";
        //        Upsrequest = Upsrequest + "<v2:TrackRequest>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "<v11:Request>";
        //        Upsrequest = Upsrequest + "<v11:RequestOption>15</v11:RequestOption>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "<v11:SubVersion>1801</v11:SubVersion>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "<v11:TransactionReference>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "<v11:CustomerContext>Your Test Case Summary Description</v11:CustomerContext>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "</v11:TransactionReference>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "</v11:Request>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "<v2:InquiryNumber>" + TrackingNo + "</v2:InquiryNumber>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "</v2:TrackRequest>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "</soapenv:Body>" + System.Environment.NewLine;
        //        Upsrequest = Upsrequest + "</soapenv:Envelope>";

        //        try
        //        {
        //            string mydocpath1 = LogPath;// "C:\\Processweaver\\BackUp";


        //            using (System.IO.StreamWriter outfile =
        //              new System.IO.StreamWriter(mydocpath1 + @"\UPSTrackRequest.xml"))
        //            {
        //                outfile.Write(Upsrequest);
        //                outfile.Close();
        //                outfile.Dispose();
        //                //GC.Collect();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //        ServicePointManager.Expect100Continue = true;
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //        HttpWebRequest myRequest1 = (HttpWebRequest)HttpWebRequest.Create("https://onlinetools.ups.com/webservices/Track");
        //        myRequest1.AllowAutoRedirect = false;
        //        myRequest1.Method = "POST";
        //        myRequest1.ContentType = "text/xml;charset=UTF-8";
        //        Stream RequestStream1 = myRequest1.GetRequestStream();
        //        byte[] SomeBytes1 = Encoding.UTF8.GetBytes(Upsrequest);
        //        RequestStream1.Write(SomeBytes1, 0, SomeBytes1.Length);
        //        RequestStream1.Close();
        //        RequestStream1.Dispose();



        //        // ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
        //        HttpWebResponse myResponse = (HttpWebResponse)myRequest1.GetResponse();
        //        if (myResponse.StatusCode == HttpStatusCode.OK)
        //        {
        //            Stream ReceiveStream = myResponse.GetResponseStream();
        //            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
        //            StreamReader readStream = new StreamReader(ReceiveStream, encode);
        //            string Result = readStream.ReadToEnd();
        //            readStream.Close();
        //            readStream.Dispose();
        //            Result = Result.Replace("soapenv:", "");
        //            Result = Result.Replace("common:", "");
        //            Result = Result.Replace("trk:", "");
        //            Result = Result.Replace("err:", "");
        //            try
        //            {
        //                string mydocpath1 = LogPath;//"C:\\Processweaver\\BackUp";


        //                using (System.IO.StreamWriter outfile =
        //                  new System.IO.StreamWriter(mydocpath1 + @"\UPSTrackResponse.xml"))
        //                {
        //                    outfile.Write(Result);
        //                    outfile.Close();
        //                    outfile.Dispose();
        //                    //GC.Collect();
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //            }
        //            MyxmlDocument.LoadXml(Result);
        //            status = MyxmlDocument.GetElementsByTagName("Description")[0].InnerText;



        //            if (status == "Failure")
        //            {
        //                //addquery("Failure message from UPS" + MyxmlDocument.GetElementsByTagName("ErrorDescription")[0].InnerText);
        //            }
        //            else
        //            {
        //                System.Xml.XmlNodeList pkg_NodeList1 = null;
        //                pkg_NodeList1 = MyxmlDocument.SelectNodes("/Envelope/Body/TrackResponse/Shipment/Package");
        //                if (pkg_NodeList1.Count > 0)
        //                {
        //                    for (int t = 0; t <= pkg_NodeList1.Count - 1; t++)
        //                    {
        //                        if (pkg_NodeList1[t].SelectSingleNode("TrackingNumber").InnerText == TrackingNo)
        //                        {

        //                            System.Xml.XmlNodeList ActivityNodeList = pkg_NodeList1[t].SelectNodes("Activity");
        //                            if (ActivityNodeList.Count > 0)
        //                            {
        //                                //deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);

        //                                //for (int k = 0; k <= ActivityNodeList.Count - 1; k++)
        //                                //{
        //                                string track_podsignature = "";
        //                                string track_podLocation = "";
        //                                DateTime? dtPODDateTime = null;
        //                                string track_PODStatus = "";

        //                                int k = 0;
        //                                try
        //                                {
        //                                    track_podsignature = ActivityNodeList[k].SelectSingleNode("ActivityLocation/SignedForByName").InnerText;
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    track_podsignature = "";
        //                                }
        //                                string statusdate = "";
        //                                try
        //                                {
        //                                    statusdate = ActivityNodeList[k].SelectSingleNode("Date").InnerText;
        //                                    string year = statusdate.Substring(0, 4);
        //                                    string month = statusdate.Substring(4, 2);
        //                                    string day = statusdate.Substring(6, 2);
        //                                    statusdate = year + "-" + month + "-" + day;
        //                                }
        //                                catch (Exception ex)
        //                                {

        //                                }
        //                                string statustime_1 = "";
        //                                try
        //                                {
        //                                    statustime_1 = ActivityNodeList[k].SelectSingleNode("Time").InnerText;
        //                                    string hour = statustime_1.Substring(0, 2);
        //                                    string minute = statustime_1.Substring(2, 2);
        //                                    string sec = statustime_1.Substring(4, 2);
        //                                    statustime_1 = hour + ":" + minute + ":" + sec;
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    statustime_1 = "00:00:00";
        //                                }
        //                                string upsstatustime = statusdate + " " + statustime_1;
        //                                try
        //                                {
        //                                    dtPODDateTime = DateTime.ParseExact(upsstatustime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    addquery("upsstatustime :" + upsstatustime);
        //                                    addquery("date Exp : " + ex.ToString());
        //                                }

        //                                string StateProvinceCode = "";
        //                                try
        //                                {
        //                                    StateProvinceCode = ActivityNodeList[k].SelectSingleNode("ActivityLocation/Address/StateProvinceCode").InnerText;
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    StateProvinceCode = "";
        //                                }
        //                                string CountryCode = "";
        //                                try
        //                                {
        //                                    CountryCode = ActivityNodeList[k].SelectSingleNode("ActivityLocation/Address/CountryCode").InnerText;
        //                                    if (CountryCode != "")
        //                                    {
        //                                        DataRow[] dataCCRows = dsCountryCode.Tables[0].Select("COUNTRY_CODE='" + CountryCode + "'");// to get items by filter package number
        //                                        if (dataCCRows.Length > 0)
        //                                        {
        //                                            CountryCode = dataCCRows[0]["COUNTRY"].ToString();
        //                                        }
        //                                    }
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    CountryCode = "";
        //                                }
        //                                string City = "";
        //                                try
        //                                {
        //                                    City = ActivityNodeList[k].SelectSingleNode("ActivityLocation/Address/City").InnerText;
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    City = "";
        //                                }
        //                                try
        //                                {
        //                                    if (City != "" && StateProvinceCode != "" && CountryCode != "")
        //                                        track_podLocation = City + ", " + StateProvinceCode + ", " + CountryCode;
        //                                    else if (CountryCode != "")
        //                                    {
        //                                        track_podLocation = CountryCode;
        //                                    }
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    track_podLocation = "";
        //                                }

        //                                try
        //                                {
        //                                    track_PODStatus = ActivityNodeList[k].SelectSingleNode("Status/Description").InnerText;
        //                                    if (track_PODStatus == "Delivered")
        //                                        track_PODStatus = "DELIVERED";
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    track_PODStatus = "";
        //                                }
        //                                string strStatus_code = "";
        //                                try
        //                                {
        //                                    //strStatus_code = UPSSTATUS.Tables["StatusCode"].Rows[0]["Code"].ToString();
        //                                    if (ActivityNodeList[k].SelectSingleNode("Status/Type").InnerText.ToUpper() == "D")
        //                                    {
        //                                        strStatus_code = "DEL";
        //                                    }
        //                                    else
        //                                    {
        //                                        strStatus_code = "INT";

        //                                    }

        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    strStatus_code = "";
        //                                }

        //                                try
        //                                {
        //                                    if (ActivityNodeList[k].SelectSingleNode("Status/Type").InnerText.ToUpper() == "D" && pkg_NodeList1[t].SelectSingleNode("DeliveryIndicator").InnerText == "Y")
        //                                    {


        //                                        XmlNodeList UPSDocumentList = ActivityNodeList[k].SelectNodes("Document");
        //                                        if (UPSDocumentList.Count > 0)
        //                                        {
        //                                            for (int d = 0; d <= UPSDocumentList.Count - 1; d++)
        //                                            {
        //                                                if (UPSDocumentList[d].SelectSingleNode("Type/Description").InnerText == "POD Letter")
        //                                                {
        //                                                    string FileName = LabelPath + "\\" + TrackingNo + ".html";

        //                                                    byte[] base64byte = Convert.FromBase64String(UPSDocumentList[d].SelectSingleNode("Content").InnerText);
        //                                                    string decodedText = null;
        //                                                    decodedText = Encoding.UTF8.GetString(base64byte);

        //                                                    FileStream obj = File.Create(FileName);
        //                                                    obj.Write(base64byte, 0, base64byte.Length);
        //                                                    obj.Close();
        //                                                    obj.Dispose();
        //                                                    base64byte = null;
        //                                                }
        //                                                else if (UPSDocumentList[d].SelectSingleNode("Type/Description").InnerText == "Signature Image")
        //                                                {
        //                                                    string FileName = LabelPath + "\\POD_" + TrackingNo + ".gif";

        //                                                    byte[] base64byte = Convert.FromBase64String(UPSDocumentList[d].SelectSingleNode("Content").InnerText);
        //                                                    string decodedText = null;
        //                                                    decodedText = Encoding.UTF8.GetString(base64byte);

        //                                                    FileStream obj = File.Create(FileName);
        //                                                    obj.Write(base64byte, 0, base64byte.Length);
        //                                                    obj.Close();
        //                                                    obj.Dispose();
        //                                                    base64byte = null;
        //                                                }
        //                                            }
        //                                        }

        //                                    }


        //                                }
        //                                catch (Exception)
        //                                {

        //                                }

        //                                //if(k==0)
        //                                //{
        //                                XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);
        //                                //}
        //                                //else
        //                                //{
        //                                //    XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code,k.ToString(), MastertrackingNo);
        //                                //}


        //                                if (ERPUpdateFlag.ToUpper() == "TRUE" && FeederSystemName != "Manual" && k.ToString() == "0")
        //                                {
        //                                    ERPUPDATE(erpenginepath, PlantID, CustomerID, TrackingNo, track_PODStatus, track_podsignature, dtPODDateTime, ERPName, DeliveryNUM);
        //                                }


        //                                // }

        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        //System.Console.WriteLine("INSIDE UPS POD EXCEPTION: " + ex.Message);
        //        addquery("UPS Error: " + ex.ToString());
        //    }

        //}


        //  string carrier, string ERPName, string DeliveryNUM, string FeederSystemName, string MastertrackingNo)
        public static void PUROLATORESHIP_Status(object pparam_obj)
        {

            DataSet PurolatorDS = new DataSet();
            string PuroUserid = "", PuroPassword = "", PuroMeter = "", AuthHeader = "", TrackingNo = "", shippingno = "";
            PurolatorDS.Clear();

            try
            {
                Array argArray = new object[2];
                argArray = (Array)pparam_obj;

                string AccountNo = (string)argArray.GetValue(0);
                TrackingNo = (string)argArray.GetValue(1);
                string PlantID = (string)argArray.GetValue(2);
                string Service = (string)argArray.GetValue(3);
                string connectionstring = (string)argArray.GetValue(4);
                string strShipFromCountry = (string)argArray.GetValue(5);
                string strDate = (string)argArray.GetValue(6);
                shippingno = (string)argArray.GetValue(7);
                string erpenginepath = (string)argArray.GetValue(8);
                string ERPUpdateFlag = (string)argArray.GetValue(9);
                string LabelPath = (string)argArray.GetValue(10);
                DataSet dsCarrier = (DataSet)argArray.GetValue(11);
                SqlConnection con = (SqlConnection)argArray.GetValue(12);
                string CustomerID = (string)argArray.GetValue(13);
                string carrier = (string)argArray.GetValue(14);
                string ERPName = (string)argArray.GetValue(15);
                string DeliveryNUM = (string)argArray.GetValue(16);
                string FeederSystemName = (string)argArray.GetValue(17);
                string MastertrackingNo = (string)argArray.GetValue(18);

                DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number


                if (dataRows.Length > 0)
                {
                    PuroUserid = dataRows[0]["USER_ID"].ToString();
                    PuroPassword = dataRows[0]["PASSWORD"].ToString();
                    PuroMeter = dataRows[0]["METER_NUMBER"].ToString();

                }




                string resultxml = "";
                resultxml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:v1=\"http://purolator.com/pws/datatypes/v1\">";
                resultxml = resultxml + "<soapenv:Header>";
                resultxml = resultxml + "<v1:RequestContext>";
                resultxml = resultxml + "<v1:Version>1.2</v1:Version>";
                resultxml = resultxml + "<v1:Language>en</v1:Language>";
                resultxml = resultxml + "<v1:GroupID>1111</v1:GroupID>";
                resultxml = resultxml + "<v1:RequestReference>Purolator</v1:RequestReference>";
                resultxml = resultxml + "<v1:UserToken>" + PuroMeter + "</v1:UserToken>";
                resultxml = resultxml + "</v1:RequestContext>";
                resultxml = resultxml + "</soapenv:Header>";
                resultxml = resultxml + "<soapenv:Body>";
                resultxml = resultxml + "<v1:TrackPackagesByPinRequest>";
                resultxml = resultxml + "<v1:PINs>";
                resultxml = resultxml + "<v1:PIN>";
                resultxml = resultxml + "<v1:Value>" + TrackingNo + "</v1:Value>";
                resultxml = resultxml + "</v1:PIN>";
                resultxml = resultxml + "</v1:PINs>";
                resultxml = resultxml + "</v1:TrackPackagesByPinRequest>";
                resultxml = resultxml + "</soapenv:Body>";
                resultxml = resultxml + "</soapenv:Envelope>";

                try
                {
                    using (System.IO.StreamWriter outfile =
                    new System.IO.StreamWriter(LogPath + @"\PUROLATORESHIP_TrackRequest.xml"))
                    {
                        outfile.Write(resultxml);
                        outfile.Close();
                        outfile.Dispose();
                        //GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                    addquery(ex.ToString());
                }

                try
                {

                    try
                    {
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                    }
                    catch (Exception ex)
                    {

                    }

                    AuthHeader = PuroUserid + ":" + PuroPassword;
                    byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes(AuthHeader);
                    string encodedAuthHeaderText = Convert.ToBase64String(bytesToEncode);
                    HttpWebRequest myRequestPurolator = (HttpWebRequest)HttpWebRequest.Create("https://webservices.purolator.com/EWS/V1/Tracking/TrackingService.asmx");
                    myRequestPurolator.AllowAutoRedirect = false;
                    myRequestPurolator.Method = "POST";
                    myRequestPurolator.ContentType = "text/xml;charset=UTF-8";
                    // myRequestPurolator.Headers.Add("Authorization", encodedAuthHeaderText);
                    myRequestPurolator.Headers.Add("Authorization", "Basic " + encodedAuthHeaderText);
                    myRequestPurolator.Headers.Add("SOAPAction", "http://purolator.com/pws/service/v1/TrackPackagesByPin");

                    myRequestPurolator.PreAuthenticate = true;

                    // Create post stream
                    Stream RequestStream = myRequestPurolator.GetRequestStream();
                    byte[] SomeBytes1 = Encoding.UTF8.GetBytes(resultxml);
                    RequestStream.Write(SomeBytes1, 0, SomeBytes1.Length);
                    RequestStream.Close();


                    XmlDocument xmlDoc = new XmlDocument();

                    try
                    {
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                    }
                    catch (Exception ex)
                    {
                    }

                    HttpWebResponse myResponse = (HttpWebResponse)myRequestPurolator.GetResponse();
                    if (myResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Stream ReceiveStream = myResponse.GetResponseStream();
                        Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

                        StreamReader readStream = new StreamReader(ReceiveStream, encode);

                        string Result = readStream.ReadToEnd();
                        readStream.Close();
                        readStream.Dispose();
                        System.Xml.XmlDocument MyXMLDocument;
                        MyXMLDocument = new System.Xml.XmlDocument();
                        MyXMLDocument.LoadXml(Result);
                        PurolatorDS.ReadXml(new XmlTextReader(new System.IO.StringReader(MyXMLDocument.InnerXml)));

                        try
                        {
                            using (System.IO.StreamWriter outfile =
                            new System.IO.StreamWriter(LogPath + @"\PUROLATORESHIP_TrackResponse.xml"))
                            {
                                outfile.Write(Result);
                                outfile.Close();
                                outfile.Dispose();
                                //GC.Collect();
                            }
                        }
                        catch (Exception ex)
                        {
                            addquery(ex.ToString());
                        }

                        string track_podLocation = "";
                        DateTime? dtPODDateTime = null;
                        string track_PODStatus = "";
                        string strStatus_code = "";
                        int k = 0;
                        string statusdatetime = "";
                        string statustime = "";
                        string statusdate = "";
                        DateTime time;
                        if (PurolatorDS.Tables["Scan"].Rows.Count > 0)
                        {
                            try
                            {
                                statusdate = PurolatorDS.Tables["Scan"].Rows[k]["ScanDate"].ToString().Trim();
                            }
                            catch (Exception ex)
                            {
                            }
                            try
                            {
                                statustime = PurolatorDS.Tables["Scan"].Rows[k]["ScanTime"].ToString().Trim();

                                TimeSpan timeSpan = TimeSpan.ParseExact(statustime, "hhmmss", null);
                                time = DateTime.Today.Add(timeSpan);
                                statusdatetime = statusdate + " " + time.ToString("hh:mm:ss");

                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {

                                dtPODDateTime = Convert.ToDateTime(statusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                track_podLocation = PurolatorDS.Tables["Depot"].Rows[k]["Name"].ToString();

                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                string pstrInsertQuery = "";
                                track_PODStatus = PurolatorDS.Tables["Scan"].Rows[k]["Description"].ToString().Trim();
                                if (track_PODStatus.ToUpper() == "SHIPMENT DELIVERED")
                                {
                                    track_PODStatus = "DELIVERED";
                                    strStatus_code = "DEL";
                                    deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);// ===== Deleting the existed record.
                                    System.Threading.Thread.Sleep(100);
                                    if (PurolatorDS.Tables["Scan"].Rows.Count > 0)
                                    {
                                        pstrInsertQuery = "insert into[XCARRIER_SHIPPING_VISIBILITY] (shipping_num, POD_DATETIME, POD_SIGN, POD_STATUS, Tracking_num, StatusCode, CURRENT_PLACE, CARRIER) values ";
                                    }

                                    for (int i = 0; i < PurolatorDS.Tables["Scan"].Rows.Count; i++)
                                    {
                                        string ptrack_podLocation = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrStatus_code = "", ptrack_podsignature = "", pstatusdate = "", pstatustime = "";
                                        DateTime? pdtPODDateTime = null;
                                        DateTime ptime;

                                        try
                                        {
                                            pstatusdate = PurolatorDS.Tables["Scan"].Rows[i]["ScanDate"].ToString().Trim();
                                        }
                                        catch (Exception ex)
                                        {
                                        }

                                        try
                                        {
                                            pstatustime = PurolatorDS.Tables["Scan"].Rows[i]["ScanTime"].ToString().Trim();

                                            TimeSpan ptimeSpan = TimeSpan.ParseExact(statustime, "hhmmss", null);
                                            ptime = DateTime.Today.Add(ptimeSpan);
                                            pstatusdatetime = statusdate + " " + ptime.ToString("hh:mm:ss");


                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            pdtPODDateTime = Convert.ToDateTime(pstatusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            ptrack_PODStatus = PurolatorDS.Tables["Scan"].Rows[i]["Description"].ToString().Trim();
                                            if (ptrack_PODStatus.ToUpper() == "SHIPMENT DELIVERED")
                                            {
                                                ptrack_PODStatus = "Delivered";
                                                pstrStatus_code = "DEL";
                                            }
                                            else
                                            {
                                                // ptrack_PODStatus = "In-Transit";
                                                pstrStatus_code = "INT";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ptrack_PODStatus = "";
                                        }

                                        string pstrDatetime = null;
                                        try
                                        {
                                            pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            ptrack_podLocation = PurolatorDS.Tables["Depot"].Rows[i]["Name"].ToString();

                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        pstrInsertQuery = pstrInsertQuery + "('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'),";

                                    }
                                    if (pstrInsertQuery != "")
                                    {
                                        try
                                        {
                                            pstrInsertQuery = pstrInsertQuery.Substring(0, pstrInsertQuery.Length - 1);

                                            SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                            if (con.State == ConnectionState.Closed)
                                            {
                                                con.Open();
                                            }
                                            SqlCmd.ExecuteNonQuery();

                                            if (con.State == ConnectionState.Open)
                                            {
                                                con.Close();
                                            }

                                            System.Threading.Thread.Sleep(100);
                                        }
                                        catch (Exception ex)
                                        {

                                            if (con.State == ConnectionState.Open)
                                                con.Close();

                                        }
                                    }



                                }
                                else
                                {
                                    strStatus_code = "INT";
                                }
                            }
                            catch (Exception ex)
                            {
                                track_PODStatus = "";
                            }
                            XCARRIERUPDATE(con, dtPODDateTime, "", track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);

                        }

                    }



                }
                catch (Exception ex)
                {
                    addquery("Unable run PUROLATORESHIP for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
                }

                try
                {
                    if (PurolatorDS != null)
                    {
                        PurolatorDS.Dispose();
                    }
                }
                catch (Exception ex)
                {


                }

            }
            catch (Exception ex)
            {
                addquery("Unable run PUROLATORESHIP for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
            }
        }
        public static void UPSPod_Status(object pparam_obj)
        {
            string LicenseNo = "";
            string UserId = "";
            string Password = "";
            string _accountno = "";
            string custid = "";
            string TrackingNo = "";

            string strShipTOContact = "";
            string strShipTOAdd1 = "";
            string strShipTOAdd2 = "";

            string strShipTOCity = "";
            string strShipTOState = "";
            string strShipTOZipCode = "";
            string strShipTOCountry = "";

            string shippingno = "", ERPUpdateFlag = "", ERPName = "", FeederSystemName = "", OrderTypeName = "", strShipFromCountry = "", Service = "", MastertrackingNo = "", DeliveryNUM = "", CustomerID = "", strDate = "", AccountNo = "", carrier = "", PlantID = "", connectionstring = "", erpenginepath = "", LabelPath = "";
            SqlConnection con = null;
            try
            {
                Array UPSargArray = new object[2];
                UPSargArray = (Array)pparam_obj;

                AccountNo = (string)UPSargArray.GetValue(0);
                TrackingNo = (string)UPSargArray.GetValue(1);
                PlantID = (string)UPSargArray.GetValue(2);
                Service = (string)UPSargArray.GetValue(3);
                connectionstring = (string)UPSargArray.GetValue(4);
                strShipFromCountry = (string)UPSargArray.GetValue(5);
                strDate = (string)UPSargArray.GetValue(6);
                shippingno = (string)UPSargArray.GetValue(7);
                erpenginepath = (string)UPSargArray.GetValue(8);
                ERPUpdateFlag = (string)UPSargArray.GetValue(9);
                LabelPath = (string)UPSargArray.GetValue(10);
                DataSet dsCarrier = (DataSet)UPSargArray.GetValue(11);
                con = (SqlConnection)UPSargArray.GetValue(12);
                CustomerID = (string)UPSargArray.GetValue(13);
                carrier = (string)UPSargArray.GetValue(14);
                ERPName = (string)UPSargArray.GetValue(15);
                DeliveryNUM = (string)UPSargArray.GetValue(16);
                FeederSystemName = (string)UPSargArray.GetValue(17);
                MastertrackingNo = (string)UPSargArray.GetValue(18);
                OrderTypeName = (string)UPSargArray.GetValue(19);

                strShipTOContact = (string)UPSargArray.GetValue(20);
                strShipTOAdd1 = (string)UPSargArray.GetValue(21);
                strShipTOAdd2 = (string)UPSargArray.GetValue(22);

                strShipTOCity = (string)UPSargArray.GetValue(23);
                strShipTOState = (string)UPSargArray.GetValue(24);
                strShipTOZipCode = (string)UPSargArray.GetValue(25);
                strShipTOCountry = (string)UPSargArray.GetValue(26);


                DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number



                if (dataRows.Length > 0)
                {
                    LicenseNo = dataRows[0]["LICENSE_NUMBER"].ToString();
                    UserId = dataRows[0]["User_ID"].ToString();
                    Password = dataRows[0]["Password"].ToString();
                    _accountno = dataRows[0]["Account_Number"].ToString();
                    custid = dataRows[0]["COMPANY_ID"].ToString();
                }
                //LicenseNo = "";
                //UserId = "";
                //Password = "";

                //TrackingNo = "";
            }
            catch (Exception)
            {

            }

            try
            {

                DateTime dtShipDate = DateTime.Now;
                string Upsrequest = "";
                string status = "";

                Upsrequest = "";
                Upsrequest = Upsrequest + "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:v1=\"http://www.ups.com/XMLSchema/XOLTWS/UPSS/v1.0\" xmlns:v2=\"http://www.ups.com/XMLSchema/XOLTWS/Track/v2.0\" xmlns:v11=\"http://www.ups.com/XMLSchema/XOLTWS/Common/v1.0\">" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<soapenv:Header>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v1:UPSSecurity>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v1:UsernameToken>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v1:Username>" + UserId + "</v1:Username>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v1:Password>" + Password + "</v1:Password>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</v1:UsernameToken>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v1:ServiceAccessToken>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v1:AccessLicenseNumber>" + LicenseNo + "</v1:AccessLicenseNumber>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</v1:ServiceAccessToken>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</v1:UPSSecurity>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</soapenv:Header>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<soapenv:Body>";
                Upsrequest = Upsrequest + "<v2:TrackRequest>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v11:Request>";
                Upsrequest = Upsrequest + "<v11:RequestOption>15</v11:RequestOption>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v11:SubVersion>1801</v11:SubVersion>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v11:TransactionReference>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v11:CustomerContext>Your Test Case Summary Description</v11:CustomerContext>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</v11:TransactionReference>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</v11:Request>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v2:InquiryNumber>" + TrackingNo + "</v2:InquiryNumber>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</v2:TrackRequest>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</soapenv:Body>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</soapenv:Envelope>";


                try
                {
                    using (System.IO.StreamWriter outfile =
                    new System.IO.StreamWriter(LogPath + @"\UPS_TrackRequest.xml"))
                    {
                        outfile.Write(Upsrequest);
                        outfile.Close();
                        outfile.Dispose();
                        //GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                    addquery(ex.ToString());
                }

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://onlinetools.ups.com/webservices/Track");

                request.Method = "POST";
                request.AllowAutoRedirect = false;

                byte[] bytes = Encoding.UTF8.GetBytes(Upsrequest);
                request.ContentLength = bytes.Length;
                request.ContentType = "text/xml;charset=UTF-8";

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                    requestStream.Dispose();

                }

                XmlDocument xmlDoc = new XmlDocument();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse();
                if (myResponse.StatusCode == HttpStatusCode.OK)
                {
                    Stream ReceiveStream = myResponse.GetResponseStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader readStream = new StreamReader(ReceiveStream, encode);
                    string Result = readStream.ReadToEnd();
                    readStream.Close();
                    readStream.Dispose();

                    encode = null;

                    ReceiveStream.Close();
                    ReceiveStream.Dispose();

                    myResponse.Close();
                    myResponse.Dispose();
                    bytes = null;

                    request.Abort();

                    Result = Result.Replace("soapenv:", "");
                    Result = Result.Replace("common:", "");
                    Result = Result.Replace("trk:", "");
                    Result = Result.Replace("err:", "");
                    xmlDoc.LoadXml(Result.Trim());
                    try
                    {
                        using (System.IO.StreamWriter outfile =
                        new System.IO.StreamWriter(LogPath + @"\UPS_TrackResponse.xml"))
                        {
                            outfile.Write(Result);
                            outfile.Close();
                            outfile.Dispose();
                            //GC.Collect();
                        }
                    }
                    catch (Exception ex)
                    {
                        addquery(ex.ToString());
                    }


                    // XmlNodeList nodelist = xmlDoc.SelectNodes("/Envelope/Body/TrackResponse/Shipment/Activity");
                    XmlNodeList nodelist = xmlDoc.SelectNodes("/Envelope/Body/TrackResponse/Shipment/Package");
                    if (nodelist.Count > 0)
                    {

                        string track_podsignature = "";
                        string track_podLocation = "";
                        DateTime? dtPODDateTime = null;
                        string track_PODStatus = "";
                        string track_PODCity = "", track_PODState = "", track_PODCountry = "", track_PODZIPCode = "";
                        string strTrackNotes = "";
                        int k = 0;
                        for (int p = 0; p < nodelist.Count; p++)
                        {
                            if (TrackingNo == nodelist[p].SelectSingleNode("TrackingNumber").InnerText)
                            {
                                k = p;
                                break;
                            }

                        }
                        string statusdate = "", statusdatetime = "";
                        try
                        {
                            statusdate = nodelist[k].SelectSingleNode("Activity/Date").InnerText;
                            string year = statusdate.Substring(0, 4);
                            string month = statusdate.Substring(4, 2);
                            string day = statusdate.Substring(6, 2);
                            statusdate = year + "-" + month + "-" + day;

                            statusdatetime = nodelist[k].SelectSingleNode("Activity/Time").InnerText;
                            string hour = statusdatetime.Substring(0, 2);
                            string minute = statusdatetime.Substring(2, 2);
                            string sec = statusdatetime.Substring(4, 2);
                            statusdatetime = hour + ":" + minute + ":" + sec;
                        }
                        catch (Exception ex) { }

                        try
                        {
                            dtPODDateTime = DateTime.ParseExact(statusdate + " " + statusdatetime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                        }
                        catch (Exception ex) { }
                        try
                        {
                            track_podsignature = xmlDoc.GetElementsByTagName("SignedForByName")[0].InnerText;
                        }
                        catch (Exception exp) { }
                        try
                        {
                            track_podLocation = nodelist[k].SelectSingleNode("Activity/ActivityLocation/Address/City").InnerText + ", " + nodelist[k].SelectSingleNode("Activity/ActivityLocation/Address/StateProvinceCode").InnerText;
                        }
                        catch (Exception exp) { }

                        try
                        {
                            track_PODCity = nodelist[k].SelectSingleNode("Activity/ActivityLocation/Address/City").InnerText;
                        }
                        catch (Exception ex)
                        {
                        }
                        try
                        {
                            track_PODState = nodelist[k].SelectSingleNode("Activity/ActivityLocation/Address/StateProvinceCode").InnerText;
                        }
                        catch (Exception ex) { }
                        try
                        {
                            track_PODCountry = nodelist[k].SelectSingleNode("Activity/ActivityLocation/Address/CountryCode").InnerText;
                        }
                        catch (Exception ex) { }
                        try
                        {
                            track_PODZIPCode = nodelist[k].SelectSingleNode("Activity/ActivityLocation/Address/PostalCode").InnerText;
                        }
                        catch (Exception ex) { }
                        try
                        {
                            strTrackNotes = nodelist[k].SelectSingleNode("Activity/ActivityLocation/Description").InnerText;
                        }
                        catch (Exception ex) { }
                        try
                        {
                            track_PODStatus = nodelist[k].SelectSingleNode("Activity/Status/Description").InnerText;
                            if (track_PODStatus == "Delivered")
                            {
                                track_PODStatus = "DELIVERED";
                            }
                        }
                        catch (Exception ex) { track_PODStatus = ""; }
                        string strStatus_code = "";
                        string pstrInsertQuery = "";

                        try
                        {
                            if (track_PODStatus == "DELIVERED")
                            {
                                strStatus_code = "DEL";
                                deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);

                                System.Threading.Thread.Sleep(100);

                                //for (int i = 0; i < nodelist.Count; i++)
                                //{
                                System.Xml.XmlNodeList ActivityNodeList = nodelist[k].SelectNodes("Activity");
                                if (ActivityNodeList.Count > 0)
                                {
                                    pstrInsertQuery = "insert into[XCARRIER_SHIPPING_VISIBILITY] (shipping_num, POD_DATETIME, POD_SIGN, POD_STATUS, Tracking_num, StatusCode, CURRENT_PLACE, CARRIER) values ";

                                    for (int J = 0; J < ActivityNodeList.Count; J++)
                                    {

                                        string pstatusdate = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrStatus_code = "", ptrack_podLocation = "", ptrack_podsignature = "";
                                        DateTime? pdtPODDateTime = null;

                                        try
                                        {
                                            pstatusdate = ActivityNodeList[J].SelectSingleNode("Date").InnerText;
                                            string year = pstatusdate.Substring(0, 4);
                                            string month = pstatusdate.Substring(4, 2);
                                            string day = pstatusdate.Substring(6, 2);
                                            pstatusdate = year + "-" + month + "-" + day;

                                            pstatusdatetime = ActivityNodeList[J].SelectSingleNode("Time").InnerText;
                                            string hour = pstatusdatetime.Substring(0, 2);
                                            string minute = pstatusdatetime.Substring(2, 2);
                                            string sec = pstatusdatetime.Substring(4, 2);
                                            pstatusdatetime = hour + ":" + minute + ":" + sec;

                                        }
                                        catch (Exception ex) { }

                                        try
                                        {
                                            pdtPODDateTime = DateTime.ParseExact(pstatusdate + " " + pstatusdatetime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                        }
                                        catch (Exception ex) { }
                                        try
                                        {
                                            //ptrack_PODStatus = nodelist[k].SelectSingleNode("/Envelope/Body/TrackResponse/Shipment/CurrentStatus/Description").InnerText;
                                            ptrack_PODStatus = ActivityNodeList[J].SelectSingleNode("Status/Description").InnerText;

                                            if (ptrack_PODStatus == "Delivered")
                                            {
                                                ptrack_PODStatus = "Delivered";
                                                pstrStatus_code = "DEL";
                                            }
                                            else { pstrStatus_code = "INT"; }
                                        }
                                        catch (Exception ex) { ptrack_PODStatus = ""; }

                                        string pstrDatetime = null;
                                        try
                                        {
                                            pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                        }
                                        catch (Exception ex) { }
                                        try
                                        {

                                            ptrack_podLocation = ActivityNodeList[J].SelectSingleNode("ActivityLocation/Address/City").InnerText + ", " + ActivityNodeList[J].SelectSingleNode("ActivityLocation/Address/StateProvinceCode").InnerText;
                                            ptrack_podsignature = xmlDoc.GetElementsByTagName("SignedForByName")[0].InnerText;
                                        }
                                        catch (Exception exp) { }

                                        pstrInsertQuery = pstrInsertQuery + "('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'),";


                                    }
                                    if (pstrInsertQuery != "")
                                    {
                                        try
                                        {
                                            pstrInsertQuery = pstrInsertQuery.Substring(0, pstrInsertQuery.Length - 1);
                                            addquery("UPS pstrInsertQuery :: " + pstrInsertQuery + ":: TrackingNumber" + TrackingNo);

                                            SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                            if (con.State == ConnectionState.Closed)
                                                con.Open();

                                            SqlCmd.ExecuteNonQuery();

                                            if (con.State == ConnectionState.Open)
                                                con.Close();

                                            System.Threading.Thread.Sleep(100);
                                        }
                                        catch (Exception ex)
                                        {
                                            addquery("UPS Ship Visibility Insert Exception :: TrackingNumber" + TrackingNo + " ::" + ex.ToString());


                                            if (con.State == ConnectionState.Open)
                                                con.Close();
                                        }
                                    }

                                }
                                else
                                {
                                    strStatus_code = "INT";
                                }
                                try
                                {
                                    if (ActivityNodeList[0].SelectSingleNode("Status/Type").InnerText.ToUpper() == "D" && nodelist[k].SelectSingleNode("DeliveryIndicator").InnerText == "Y")
                                    {
                                        XmlNodeList UPSDocumentList = ActivityNodeList[0].SelectNodes("Document");
                                        if (UPSDocumentList.Count > 0)
                                        {
                                            for (int d = 0; d <= UPSDocumentList.Count - 1; d++)
                                            {
                                                if (UPSDocumentList[d].SelectSingleNode("Type/Description").InnerText == "POD Letter")
                                                {
                                                    string FileName = LabelPath + "\\" + TrackingNo + ".html";

                                                    byte[] base64byte = Convert.FromBase64String(UPSDocumentList[d].SelectSingleNode("Content").InnerText);
                                                    string decodedText = null;
                                                    decodedText = Encoding.UTF8.GetString(base64byte);

                                                    FileStream obj = File.Create(FileName);
                                                    obj.Write(base64byte, 0, base64byte.Length);
                                                    obj.Close();
                                                    obj.Dispose();
                                                    base64byte = null;
                                                }
                                                else if (UPSDocumentList[d].SelectSingleNode("Type/Description").InnerText == "Signature Image")
                                                {
                                                    string FileName = LabelPath + "\\pod_" + TrackingNo + ".gif";

                                                    byte[] base64byte = Convert.FromBase64String(UPSDocumentList[d].SelectSingleNode("Content").InnerText);
                                                    string decodedText = null;
                                                    decodedText = Encoding.UTF8.GetString(base64byte);

                                                    FileStream obj = File.Create(FileName);
                                                    obj.Write(base64byte, 0, base64byte.Length);
                                                    obj.Close();
                                                    obj.Dispose();
                                                    base64byte = null;
                                                }
                                            }
                                        }

                                    }


                                }
                                catch (Exception exp)
                                {

                                }
                                //}
                            }
                            else
                            {
                                strStatus_code = "INT";
                            }
                        }
                        catch (Exception ex)
                        {
                            strStatus_code = "INT";
                        }



                        // =============== For TEST Only====
                        //track_PODStatus = "DELIVERED";
                        //strStatus_code = "DEL";
                        //dtPODDateTime = DateTime.Now;
                        // =============== For TEST Only====

                        XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);

                        if (ERPUpdateFlag.ToUpper() == "TRUE" && FeederSystemName != "Manual" && track_PODStatus == "DELIVERED")
                        {
                            ERPUPDATE(erpenginepath, PlantID, CustomerID, TrackingNo, track_PODStatus, track_podsignature, dtPODDateTime, ERPName, DeliveryNUM, carrier, shippingno,
                                FeederSystemName, OrderTypeName, strShipTOCity, strShipTOState, strShipTOCountry, strShipTOZipCode, strShipTOContact, strShipTOAdd1, strShipTOAdd2, strTrackNotes, Service);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                addquery("Unable run UPS_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
            }

        }


        public static void deleteSHIPPING_VISIBILITY(SqlConnection con, string TrackingNo, string ShippingNo)
        {
            string strInsertQuery = "";
            string strUpdateQuery = "";
            try
            {
                strInsertQuery = "delete from XCARRIER_SHIPPING_VISIBILITY where SHIPPING_NUM='" + ShippingNo + "' and Tracking_num ='" + TrackingNo + "'";

                strInsertQuery = strInsertQuery + strUpdateQuery;

                SqlCommand SqlCmd = new SqlCommand(strInsertQuery, con);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SqlCmd.ExecuteNonQuery();
                if (con.State == ConnectionState.Open)
                    con.Close();

                System.Threading.Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                //addquery("SQLUpdate Query failed for " + TrackingNo + " Reason : " + ex.Message.ToString());
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }
        public static void XCARRIERUPDATE(SqlConnection con, DateTime? PODDateTime, string PODSignature, string PODStatus, string PODLocation, string TrackingNo, string ShippingNo, string PODStatusCode, string AcitivityIndex, string MastertrackingNo, string strCarrier)
        {
            string strInsertQuery = "";
            string strUpdateQuery = "";
            string strDatetime = null;
            try
            {
                strDatetime = PODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
            }
            catch (Exception ex)
            {


            }


            try
            {
                addquery("MastertrackingNo == TrackingNo::" + MastertrackingNo + "==" + TrackingNo);
                if (MastertrackingNo == TrackingNo)
                {
                    strInsertQuery = "insert into [XCARRIER_SHIPPING_VISIBILITY] (shipping_num,POD_DATETIME,POD_SIGN,POD_STATUS,Tracking_num,StatusCode,CARRIER) values('" + ShippingNo + "','" + strDatetime + "','" + PODSignature.Replace("'", "''") + "','" + PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + PODStatusCode + "','" + strCarrier + "'); ";
                }

                if (AcitivityIndex == "0")
                {
                    strUpdateQuery = "update xcarrier_shipments set POD_DATETIME='" + strDatetime + "',POD_SIGNATURE='" + PODSignature.Replace("'", "''") + "', POD_STATUS='" + PODStatus.Replace("'", "''") + "' , TRACKING_NOTE='" + PODStatus.Replace("'", "''") + "',STATUS_CODE='" + PODStatusCode + "' where tracking_number='" + TrackingNo + "';";
                    strUpdateQuery = strUpdateQuery + "update XCARRIER_PACKAGE_MASTER set POD_DATETIME='" + strDatetime + "',POD_SIGNATURE='" + PODSignature.Replace("'", "''") + "', PODSTATUS='" + PODStatus.Replace("'", "''") + "' , POD_LOCATION='" + PODLocation.Replace("'", "''") + "' where TRACKING_NUM='" + TrackingNo + "';";
                }

                strInsertQuery = strInsertQuery + strUpdateQuery;
                if (strInsertQuery != "")
                {
                    addquery("ShipmentUpdate ::" + ":: Tracking No :" + TrackingNo + "ShipmentUpdate strInsertQuery::" + strInsertQuery);

                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    SqlCommand SqlCmd = new SqlCommand(strInsertQuery, con);
                    SqlCmd.ExecuteNonQuery();

                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }


            }
            catch (Exception ex)
            {
                addquery("strInsertQuery :" + strInsertQuery);
                addquery("SQLUpdate Query failed for " + TrackingNo + " Reason : " + ex.Message.ToString());
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }
        public static void ERPUPDATE(string erpenginepath, string PlantID, string CompanyID, string TrackingNo, string PODStatus, string PODSignature, DateTime? dtPODDate, string ERPName,
            string DeliveryNo, string strCarrierName, string strShippingno, string strFeederSystemName = "", string strOrderType = "", string strPODCity = "", string strPODState = "",
            string strPODCountry = "", string strPODZIPCode = "", string strShipTOContact = "", string strShipTOAdd1 = "", string strShipTOAdd2 = "", string strTrackNotes = "",
            string strServiceName = "")
        {
            string strxml;
            try
            {
                //DateTime dt = Convert.ToDateTime(statustime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                strxml = "<PODREQUEST>";
                strxml = strxml + "<ERP>" + strFeederSystemName + "</ERP>";
                strxml = strxml + "<PLANTID>" + PlantID + "</PLANTID>";
                strxml = strxml + "<DELIVERYNO>" + DeliveryNo + "</DELIVERYNO>";
                strxml = strxml + "<CUSTOMERID>" + CompanyID + "</CUSTOMERID>";
                strxml = strxml + "<TRACKINGNO>" + TrackingNo + "</TRACKINGNO>";

                strxml = strxml + "<OPERATION>PODUPDATE</OPERATION>";

                strxml = strxml + "<PODSTATUS>" + System.Net.WebUtility.HtmlEncode(PODStatus) + "</PODSTATUS>";
                strxml = strxml + "<PODDATE>" + dtPODDate + "</PODDATE>";
                strxml = strxml + "<PODSIGNATURE>" + System.Net.WebUtility.HtmlEncode(PODSignature) + "</PODSIGNATURE>";
                strxml = strxml + "<PODCITY>" + System.Net.WebUtility.HtmlEncode(strPODCity) + "</PODCITY>";
                strxml = strxml + "<PODSTATE>" + System.Net.WebUtility.HtmlEncode(strPODState) + "</PODSTATE>";
                strxml = strxml + "<PODZIPCODE>" + strPODZIPCode + "</PODZIPCODE>";
                strxml = strxml + "<PODCOUNTRY>" + System.Net.WebUtility.HtmlEncode(strPODCountry) + "</PODCOUNTRY>";

                strxml = strxml + "<Destination_City>" + System.Net.WebUtility.HtmlEncode(strPODCity) + "</Destination_City>";
                strxml = strxml + "<Destination_State>" + System.Net.WebUtility.HtmlEncode(strPODState) + "</Destination_State>";
                strxml = strxml + "<Destination_Zip-Code>" + strPODZIPCode + "</Destination_Zip-Code>";
                strxml = strxml + "<Destination_Country_Code>" + strPODCountry + "</Destination_Country_Code>";

                strxml = strxml + "<SHIPPINGNO>" + strShippingno + "</SHIPPINGNO>";
                strxml = strxml + "<Carrier_Name>" + strCarrierName + "</Carrier_Name>";
                strxml = strxml + "<Reference_Number>" + TrackingNo + "</Reference_Number>";
                //strxml = strxml + "<Scheduled_Delivery_Date></Scheduled_Delivery_Date>";
                strxml = strxml + "<SHIPSTATUS>" + System.Net.WebUtility.HtmlEncode(PODStatus) + "</SHIPSTATUS>";
                strxml = strxml + "<ORDERTYPE>" + strOrderType + "</ORDERTYPE>";
                strxml = strxml + "<FEEDERSYSTEM_NAME>" + strFeederSystemName + "</FEEDERSYSTEM_NAME>";

                strxml = strxml + "<Destination_Contact_Name>" + System.Net.WebUtility.HtmlEncode(strShipTOContact) + "</Destination_Contact_Name>";
                strxml = strxml + "<Destination_Address_1>" + System.Net.WebUtility.HtmlEncode(strShipTOAdd1) + "</Destination_Address_1>";
                strxml = strxml + "<Destination_Address_2>" + System.Net.WebUtility.HtmlEncode(strShipTOAdd2) + "</Destination_Address_2>";
                strxml = strxml + "<Service_Level>" + System.Net.WebUtility.HtmlEncode(strServiceName) + "</Service_Level>";
                strxml = strxml + "<Delivery_Notes>" + System.Net.WebUtility.HtmlEncode(strTrackNotes) + "</Delivery_Notes>";



                try
                {

                    if (dtPODDate.HasValue == true)
                    {
                        strxml = strxml + "<Actual_Delivery_Date>" + dtPODDate?.ToString("MM/dd/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) + "</Actual_Delivery_Date>";
                        strxml = strxml + "<Scheduled_Delivery_Date>" + dtPODDate?.ToString("MM/dd/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) + "</Scheduled_Delivery_Date>";
                        //strxml = strxml + "<Actual_Delivery_Date>" + Convert.ToDateTime(dtPODDate).ToString("MM/dd/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) + "</Actual_Delivery_Date>";
                        strxml = strxml + "<Delivery_Time>" + dtPODDate?.ToString("HHmmss") + "</Delivery_Time>";
                    }
                    else
                    {
                        strxml = strxml + "<Actual_Delivery_Date></Actual_Delivery_Date>";
                        strxml = strxml + "<Scheduled_Delivery_Date></Scheduled_Delivery_Date>";
                        strxml = strxml + "<Delivery_Time></Delivery_Time>";
                    }
                }
                catch (Exception ex)
                {
                    strxml = strxml + "<Actual_Delivery_Date></Actual_Delivery_Date>";
                    strxml = strxml + "<Scheduled_Delivery_Date></Scheduled_Delivery_Date>";
                    strxml = strxml + "<Delivery_Time></Delivery_Time>";
                }

                strxml = strxml + "<Time_Zone></Time_Zone>";
                strxml = strxml + "</PODREQUEST>";

                //addquery(strxml);
                try
                {
                    using (System.IO.StreamWriter outfile =
                    new System.IO.StreamWriter(LogPath + @"\ERPUPDATE_TrackRequest.xml"))
                    {
                        outfile.Write(strxml);
                        outfile.Close();
                        outfile.Dispose();
                        //GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                    addquery(ex.ToString());
                }

                string responce = null;
                HttpWebRequest myRequestFedEx = default(HttpWebRequest);

                myRequestFedEx = (HttpWebRequest)HttpWebRequest.Create(erpenginepath);

                myRequestFedEx.AllowAutoRedirect = false;

                myRequestFedEx.Method = "POST";
                myRequestFedEx.Headers.Add("SOAPAction", "processRequest/processRequest");
                myRequestFedEx.ContentType = "text/xml;charset=UTF-8";
                Stream RequestStreamFD = myRequestFedEx.GetRequestStream();
                byte[] SomeByte = Encoding.UTF8.GetBytes(strxml.ToString());
                RequestStreamFD.Write(SomeByte, 0, SomeByte.Length);
                RequestStreamFD.Close();
                RequestStreamFD.Dispose();
                System.Xml.XmlDocument ResponceXMLDocument;
                ResponceXMLDocument = new System.Xml.XmlDocument();
                try
                {
                    HttpWebResponse myResponseUSPS = (HttpWebResponse)myRequestFedEx.GetResponse();
                    if (myResponseUSPS.StatusCode == HttpStatusCode.OK)
                    {

                        Stream ReceiveStreamUSPS = myResponseUSPS.GetResponseStream();
                        Encoding encode1 = System.Text.Encoding.GetEncoding("utf-8");
                        StreamReader readStream1 = new StreamReader(ReceiveStreamUSPS, encode1);
                        responce = readStream1.ReadToEnd();
                        readStream1.Close();
                        readStream1.Dispose();
                        ResponceXMLDocument.LoadXml(responce);

                        try
                        {
                            using (System.IO.StreamWriter outfile =
                            new System.IO.StreamWriter(LogPath + @"\ERPUPDATE_TrackResponce.xml"))
                            {
                                outfile.Write(responce);
                                outfile.Close();
                                outfile.Dispose();
                                //GC.Collect();
                            }
                        }
                        catch (Exception ex)
                        {
                            addquery(ex.ToString());
                        }

                        //addquery("ERPUPDATE Response: " + ResponceXMLDocument.GetElementsByTagName("Message")[0].InnerText);
                    }
                }
                catch (Exception ex)
                {
                    // addquery("ERPUPDATE Failed: " + ResponceXMLDocument.GetElementsByTagName("Message")[0].InnerText);
                }
            }
            catch (Exception)
            {

            }
        }



        public string getAllDelivered(string str_trackingno, string connection)
        {
            string strstatusTrackingno = "";
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("select trackingno,(select case when count(p.TrackingNo) = 0 then 'true' else 'false' end as status from Packing p where p.ShippingNo= (select shippingno from Packing where trackingno='" + str_trackingno + "') and p.PODStatus not in ('DELIVERED') ) from Shipments where ShippingNo= convert(int,(Select ShippingNo from Packing where  trackingno='" + str_trackingno + "'))", connection);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][1].ToString().ToUpper() == "TRUE")
                    {
                        strstatusTrackingno = ds.Tables[0].Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                addquery("Exception from getalldelivered function 3001" + ex.Message.ToString());
            }
            return strstatusTrackingno;
        }

        // public static void FEDEX_Status(string AccountNo, string TrackingNo, string PlantID, string Service, string connectionstring, String strShipFromCountry, 
        //String strDate, string shippingno, string erpenginepath, string ERPUpdateFlag, string LabelPath, DataSet dsCarrier, SqlConnection con, string CustomerID, 
        //string carrier, string ERPName, string DeliveryNUM, string FeederSystemName, string MastertrackingNo)
        public static void FEDEX_Status(object pparam_obj)
        {
            string TrackingNo = "";
            string shippingno = "";

            try
            {
                Array argArray = new object[2];
                argArray = (Array)pparam_obj;

                string AccountNo = (string)argArray.GetValue(0);
                TrackingNo = (string)argArray.GetValue(1);
                string PlantID = (string)argArray.GetValue(2);
                string Service = (string)argArray.GetValue(3);
                string connectionstring = (string)argArray.GetValue(4);
                string strShipFromCountry = (string)argArray.GetValue(5);
                string strDate = (string)argArray.GetValue(6);
                shippingno = (string)argArray.GetValue(7);
                string erpenginepath = (string)argArray.GetValue(8);
                string ERPUpdateFlag = (string)argArray.GetValue(9);
                string LabelPath = (string)argArray.GetValue(10);
                DataSet dsCarrier = (DataSet)argArray.GetValue(11);
                SqlConnection con = (SqlConnection)argArray.GetValue(12);
                string CustomerID = (string)argArray.GetValue(13);
                string carrier = (string)argArray.GetValue(14);
                string ERPName = (string)argArray.GetValue(15);
                string DeliveryNUM = (string)argArray.GetValue(16);
                string FeederSystemName = (string)argArray.GetValue(17);
                string MastertrackingNo = (string)argArray.GetValue(18);
                string OrderTypeName = (string)argArray.GetValue(19);

                string strShipTOContact = (string)argArray.GetValue(20);
                string strShipTOAdd1 = (string)argArray.GetValue(21);
                string strShipTOAdd2 = (string)argArray.GetValue(22);

                string strShipTOCity = (string)argArray.GetValue(23);
                string strShipTOState = (string)argArray.GetValue(24);
                string strShipTOZipCode = (string)argArray.GetValue(25);
                string strShipTOCountry = (string)argArray.GetValue(26);


                DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number

                string fedexaccountno = "";
                string fedexmeterno = "";
                string fedexcspkey = "";
                string fedexcspwd = "";
                string fedexuserkey = "";
                string fedexuserpwd = "";
                if (dataRows.Length > 0)
                {
                    fedexaccountno = dataRows[0]["ACCOUNT_NUMBER"].ToString();
                    fedexmeterno = dataRows[0]["METER_NUMBER"].ToString();
                    fedexcspkey = dataRows[0]["CSP_USERID"].ToString();
                    fedexcspwd = dataRows[0]["CSP_PASSWORD"].ToString();
                    fedexuserkey = dataRows[0]["USER_ID"].ToString();
                    fedexuserpwd = dataRows[0]["PASSWORD"].ToString();

                }


                string strXML = "";
                strXML = strXML + "<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns='http://fedex.com/ws/track/v14'>";
                strXML = strXML + "<soapenv:Header/>";
                strXML = strXML + "<soapenv:Body>";
                strXML = strXML + "<TrackRequest>";
                strXML = strXML + "<WebAuthenticationDetail>";
                strXML = strXML + "<ParentCredential>";
                strXML = strXML + "<Key>" + fedexcspkey + "</Key>";
                strXML = strXML + "<Password>" + fedexcspwd + "</Password>";
                strXML = strXML + "</ParentCredential>";
                strXML = strXML + "<UserCredential>";
                strXML = strXML + "<Key>" + fedexuserkey + "</Key>";
                strXML = strXML + "<Password>" + fedexuserpwd + "</Password>";
                strXML = strXML + "</UserCredential>";
                strXML = strXML + "</WebAuthenticationDetail>";
                strXML = strXML + "<ClientDetail>";
                strXML = strXML + "<AccountNumber>" + fedexaccountno + "</AccountNumber>";
                strXML = strXML + "<MeterNumber>" + fedexmeterno + "</MeterNumber>";
                strXML = strXML + "</ClientDetail>";
                strXML = strXML + "<TransactionDetail>";
                strXML = strXML + "<CustomerTransactionId>***Track v14 Request using VC#***</CustomerTransactionId>";
                strXML = strXML + "</TransactionDetail>";
                strXML = strXML + "<Version>";
                strXML = strXML + "<ServiceId>trck</ServiceId>";
                strXML = strXML + "<Major>14</Major>";
                strXML = strXML + "<Intermediate>0</Intermediate>";
                strXML = strXML + "<Minor>0</Minor>";
                strXML = strXML + "</Version>";
                strXML = strXML + "<SelectionDetails>";
                strXML = strXML + "<PackageIdentifier>";
                strXML = strXML + "<Type>TRACKING_NUMBER_OR_DOORTAG</Type>";
                strXML = strXML + "<Value>" + TrackingNo + "</Value>";
                strXML = strXML + "</PackageIdentifier>";
                strXML = strXML + "</SelectionDetails>";
                strXML = strXML + "<ProcessingOptions>INCLUDE_DETAILED_SCANS</ProcessingOptions>";
                strXML = strXML + "</TrackRequest>";
                strXML = strXML + "</soapenv:Body>";
                strXML = strXML + "</soapenv:Envelope>";

                try
                {
                    using (System.IO.StreamWriter outfile =
                    new System.IO.StreamWriter(LogPath + @"\FEDEX_TrackRequest.xml"))
                    {
                        outfile.Write(strXML);
                        outfile.Close();
                        outfile.Dispose();
                        //GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                    addquery(ex.ToString());
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://ws.fedex.com/web-services/track");
                request.AllowAutoRedirect = false;

                request.Method = "POST";
                byte[] bytes = Encoding.UTF8.GetBytes(strXML);
                request.ContentLength = bytes.Length;
                request.ContentType = "text/xml;charset=UTF-8";

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                    requestStream.Dispose();

                }

                XmlDocument xmlDoc = new XmlDocument();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse();
                if (myResponse.StatusCode == HttpStatusCode.OK)
                {
                    Stream ReceiveStream = myResponse.GetResponseStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader readStream = new StreamReader(ReceiveStream, encode);
                    string Result = readStream.ReadToEnd();

                    readStream.Close();
                    readStream.Dispose();

                    encode = null;

                    ReceiveStream.Close();
                    ReceiveStream.Dispose();

                    myResponse.Close();
                    myResponse.Dispose();
                    bytes = null;

                    request.Abort();

                    Result = Result.Replace("SOAP-ENV:", "");
                    Result = Result.Replace(":SOAP-ENV", "");
                    Result = Result.Replace("<Envelope xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\">", "<Envelope>");
                    Result = Result.Replace("<TrackReply xmlns=\"http://fedex.com/ws/track/v14\">", "<TrackReply>");
                    xmlDoc.LoadXml(Result.Trim());
                    try
                    {
                        using (System.IO.StreamWriter outfile =
                        new System.IO.StreamWriter(LogPath + @"\FEDEX_TrackResponse.xml"))
                        {
                            outfile.Write(Result);
                            outfile.Close();
                            outfile.Dispose();
                            //GC.Collect();
                        }
                    }
                    catch (Exception ex)
                    {
                        addquery(ex.ToString());
                    }


                    XmlNodeList nodelist = xmlDoc.SelectNodes("Envelope/Body/TrackReply/CompletedTrackDetails/TrackDetails/Events");
                    addquery("FEDEX nodelist.Count:" + nodelist.Count.ToString() + ":: Tracking No :" + TrackingNo);

                    if (nodelist.Count > 0)
                    {
                        addquery("INSIDE FEDEX nodelist.Count:" + nodelist.Count.ToString() + ":: Tracking No :" + TrackingNo);

                        string track_podsignature = "";
                        string track_podLocation = "";
                        DateTime? dtPODDateTime = null;
                        string track_PODStatus = "";
                        string track_Notes = "";

                        string track_PODCity = "", track_PODState = "", track_PODCountry = "", track_PODZIPCode = "";


                        int k = 0;
                        string statusdate = "", statusdatetime = "";
                        try
                        {
                            statusdatetime = nodelist[k].SelectSingleNode("Timestamp").InnerText;

                        }
                        catch (Exception ex)
                        {

                        }

                        try
                        {
                            dtPODDateTime = Convert.ToDateTime(statusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                        }
                        catch (Exception ex)
                        {

                        }
                        try
                        {
                            track_podsignature = xmlDoc.SelectSingleNode("Envelope/Body/TrackReply/CompletedTrackDetails/TrackDetails/DeliverySignatureName").InnerText;
                        }
                        catch (Exception ex)
                        {

                        }

                        try
                        {
                            track_podLocation = nodelist[k].SelectSingleNode("Address/City").InnerText + ", " + nodelist[k].SelectSingleNode("Address/StateOrProvinceCode").InnerText;

                        }
                        catch (Exception ex)
                        {

                        }
                        try
                        {
                            track_PODCity = nodelist[k].SelectSingleNode("Address/City").InnerText;
                        }
                        catch (Exception ex)
                        {

                        }
                        try
                        {
                            track_PODState = nodelist[k].SelectSingleNode("Address/StateOrProvinceCode").InnerText;
                        }
                        catch (Exception ex)
                        {

                        }
                        try
                        {
                            track_PODZIPCode = nodelist[k].SelectSingleNode("Address/PostalCode").InnerText;
                        }
                        catch (Exception ex)
                        {

                        }
                        try
                        {
                            track_PODCountry = nodelist[k].SelectSingleNode("Address/CountryCode").InnerText;
                        }
                        catch (Exception ex)
                        {

                        }
                        try
                        {
                            track_Notes = nodelist[k].SelectSingleNode("StatusExceptionDescription").InnerText;
                        }
                        catch (Exception ex)
                        {

                        }
                        try
                        {
                            track_PODStatus = nodelist[k].SelectSingleNode("EventDescription").InnerText;
                            if (track_PODStatus == "Delivered")
                            {
                                track_PODStatus = "DELIVERED";
                            }
                        }
                        catch (Exception ex)
                        {
                            track_PODStatus = "";
                        }

                        try
                        {
                            if (nodelist[k].SelectSingleNode("EventType").InnerText.ToUpper() == "DL" && xmlDoc.SelectSingleNode("Envelope/Body/TrackReply/CompletedTrackDetails/TrackDetails/AvailableImages/Type").InnerText == "SIGNATURE_PROOF_OF_DELIVERY")
                            {

                                getSPODLetterRequest(AccountNo, "", TrackingNo, fedexmeterno, fedexcspkey, fedexcspwd, fedexuserkey, fedexuserpwd, LabelPath);
                                System.Threading.Thread.Sleep(100);

                            }


                        }
                        catch (Exception ex)
                        {
                            addquery("getSPODLetterRequest :: " + ex.ToString() + ":: TrackingNum::" + TrackingNo);
                        }


                        string strStatus_code = "";
                        string pstrInsertQuery = "";
                        try
                        {

                            if (track_PODStatus == "DELIVERED")
                            {
                                strStatus_code = "DEL";
                                deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);

                                System.Threading.Thread.Sleep(100);
                                if (nodelist.Count > 0)
                                {
                                    pstrInsertQuery = "insert into[XCARRIER_SHIPPING_VISIBILITY] (shipping_num, POD_DATETIME, POD_SIGN, POD_STATUS, Tracking_num, StatusCode, CURRENT_PLACE, CARRIER) values ";
                                }



                                for (int i = 0; i < nodelist.Count; i++)
                                {
                                    string ptrack_podLocation = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrStatus_code = "", ptrack_podsignature = "";

                                    DateTime? pdtPODDateTime = null;

                                    try
                                    {
                                        pstatusdatetime = nodelist[i].SelectSingleNode("Timestamp").InnerText;
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    try
                                    {
                                        pdtPODDateTime = Convert.ToDateTime(pstatusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    try
                                    {
                                        ptrack_PODStatus = nodelist[i].SelectSingleNode("EventDescription").InnerText;
                                        if (ptrack_PODStatus == "Delivered")
                                        {
                                            ptrack_PODStatus = "Delivered";
                                            pstrStatus_code = "DEL";
                                        }
                                        else
                                        {
                                            // ptrack_PODStatus = "In-Transit";
                                            pstrStatus_code = "INT";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ptrack_PODStatus = "";
                                    }

                                    string pstrDatetime = null;
                                    try
                                    {
                                        pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                    }
                                    catch (Exception ex)
                                    {


                                    }
                                    try
                                    {
                                        ptrack_podsignature = xmlDoc.SelectSingleNode("Envelope/Body/TrackReply/CompletedTrackDetails/TrackDetails/DeliverySignatureName").InnerText;

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    try
                                    {
                                        ptrack_podLocation = nodelist[i].SelectSingleNode("Address/City").InnerText + ", " + nodelist[i].SelectSingleNode("Address/StateOrProvinceCode").InnerText;

                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    pstrInsertQuery = pstrInsertQuery + "('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'),";

                                }
                                if (pstrInsertQuery != "")
                                {
                                    try
                                    {
                                        pstrInsertQuery = pstrInsertQuery.Substring(0, pstrInsertQuery.Length - 1);
                                        addquery("FEDEX pstrInsertQuery :: " + pstrInsertQuery + ":: TrackingNumber" + TrackingNo);
                                        SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                        if (con.State == ConnectionState.Closed)
                                        {
                                            con.Open();
                                        }
                                        SqlCmd.ExecuteNonQuery();

                                        if (con.State == ConnectionState.Open)
                                        {
                                            con.Close();
                                        }

                                        System.Threading.Thread.Sleep(100);
                                    }
                                    catch (Exception ex)
                                    {
                                        addquery("FEDEX Ship Visibility Insert Exception :: TrackingNumber" + TrackingNo + " ::" + ex.ToString());

                                        if (con.State == ConnectionState.Open)
                                            con.Close();

                                    }
                                }
                            }
                            else
                            {
                                strStatus_code = "INT";
                            }

                        }
                        catch (Exception ex)
                        {
                            addquery("");
                            // strStatus_code = "";
                        }
                        //=======TEST only ==========
                        //track_PODStatus = "DELIVERED";
                        //strStatus_code = "DEL";
                        //dtPODDateTime = DateTime.Now;
                        //=======TEST only ==========

                        XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);


                        if (ERPUpdateFlag.ToUpper() == "TRUE" && FeederSystemName != "Manual" && track_PODStatus == "DELIVERED")
                        {
                            ERPUPDATE(erpenginepath, PlantID, CustomerID, TrackingNo, track_PODStatus, track_podsignature, dtPODDateTime, ERPName, DeliveryNUM, carrier,
                                shippingno, FeederSystemName, OrderTypeName, strShipTOCity, strShipTOState, strShipTOCountry, strShipTOZipCode, strShipTOContact, strShipTOAdd1, strShipTOAdd2, track_Notes, Service);
                        }

                    }
                }



            }
            catch (Exception ex)
            {
                addquery("Unable run FEDEX_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
            }

        }

        public static void CANADAPOST_Status(object pparam_obj)
        {
            string TrackingNo = "";
            string shippingno = "";

            try
            {
                Array argArray = new object[2];
                argArray = (Array)pparam_obj;

                string AccountNo = (string)argArray.GetValue(0);
                TrackingNo = (string)argArray.GetValue(1);
                string PlantID = (string)argArray.GetValue(2);
                string Service = (string)argArray.GetValue(3);
                string connectionstring = (string)argArray.GetValue(4);
                string strShipFromCountry = (string)argArray.GetValue(5);
                string strDate = (string)argArray.GetValue(6);
                shippingno = (string)argArray.GetValue(7);
                string erpenginepath = (string)argArray.GetValue(8);
                string ERPUpdateFlag = (string)argArray.GetValue(9);
                string LabelPath = (string)argArray.GetValue(10);
                DataSet dsCarrier = (DataSet)argArray.GetValue(11);
                SqlConnection con = (SqlConnection)argArray.GetValue(12);
                string CustomerID = (string)argArray.GetValue(13);
                string carrier = (string)argArray.GetValue(14);
                string ERPName = (string)argArray.GetValue(15);
                string DeliveryNUM = (string)argArray.GetValue(16);
                string FeederSystemName = (string)argArray.GetValue(17);
                string MastertrackingNo = (string)argArray.GetValue(18);

                DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number

                string CanadapostAccountno = "";
                string Canadapostmeterno = "";
                string CanadapostCspkey = "";
                string CanadapostCspwd = "";
                string CanadapostUserkey = "";
                string CanadapostUserpwd = "";
                string URL = "";
                string AuthHeader = "";
                if (dataRows.Length > 0)
                {
                    CanadapostAccountno = dataRows[0]["ACCOUNT_NUMBER"].ToString();
                    Canadapostmeterno = dataRows[0]["METER_NUMBER"].ToString();
                    CanadapostCspkey = dataRows[0]["CSP_USERID"].ToString();
                    CanadapostCspwd = dataRows[0]["CSP_PASSWORD"].ToString();
                    CanadapostUserkey = dataRows[0]["USER_ID"].ToString();
                    CanadapostUserpwd = dataRows[0]["PASSWORD"].ToString();

                }
                //CanadapostUserkey = "";
                //CanadapostUserpwd = "";
                //TrackingNo = "";
                if (URL == "")
                {
                    URL = "https://soa-gw.canadapost.ca/vis/track/pin/";
                }

                if (URL.EndsWith("/"))
                {
                    URL = URL + TrackingNo + "/detail";
                }

                try
                {

                    AuthHeader = CanadapostUserkey + ":" + CanadapostUserpwd;
                    byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes(AuthHeader);
                    string encodedAuthHeaderText = Convert.ToBase64String(bytesToEncode);
                    try
                    {
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                    }
                    catch (Exception ex)
                    {

                    }
                    HttpWebRequest myRequestCanadaPost = (HttpWebRequest)HttpWebRequest.Create(URL);

                    myRequestCanadaPost.AllowAutoRedirect = false;
                    myRequestCanadaPost.Method = "GET";
                    myRequestCanadaPost.ContentType = "text/xml;charset=UTF-8";
                    myRequestCanadaPost.Headers.Add("Authorization", "Basic " + encodedAuthHeaderText);

                    try
                    {
                        using (System.IO.StreamWriter outfile =
                        new System.IO.StreamWriter(LogPath + @"\CANADAPOST_TrackRequest.xml"))
                        {
                            outfile.Write(myRequestCanadaPost + " _URL::" + URL + " _AuthHeader::" + AuthHeader);
                            outfile.Close();
                            outfile.Dispose();
                            //GC.Collect();
                        }
                    }
                    catch (Exception ex)
                    {
                        addquery(ex.ToString());
                    }

                    XmlDocument xmlDoc = new XmlDocument();
                    try
                    {
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                    }
                    catch (Exception ex)
                    {
                    }
                    HttpWebResponse myResponse = (HttpWebResponse)myRequestCanadaPost.GetResponse();
                    if (myResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Stream ReceiveStream = myResponse.GetResponseStream();
                        Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                        StreamReader readStream = new StreamReader(ReceiveStream, encode);
                        string Result = readStream.ReadToEnd();
                        readStream.Close();
                        readStream.Dispose();

                        encode = null;

                        ReceiveStream.Close();
                        ReceiveStream.Dispose();

                        myResponse.Close();
                        myResponse.Dispose();
                        bytesToEncode = null;

                        myRequestCanadaPost.Abort();

                        Result = Result.Replace("SOAP-ENV:", "");
                        Result = Result.Replace(":SOAP-ENV", "");
                        Result = Result.Replace("<Envelope xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\">", "<Envelope>");
                        // Result = Result.Replace("<TrackReply xmlns=\"http://fedex.com/ws/track/v14\">", "<TrackReply>");
                        Result = Result.Replace("<tracking-detail xmlns=\"http://www.canadapost.ca/ws/track\">", "<tracking-detail>");

                        xmlDoc.LoadXml(Result.Trim());
                        try
                        {
                            using (System.IO.StreamWriter outfile =
                            new System.IO.StreamWriter(LogPath + @"\CANADAPOST_TrackResponse.xml"))
                            {
                                outfile.Write(Result);
                                outfile.Close();
                                outfile.Dispose();
                                //GC.Collect();
                            }
                        }
                        catch (Exception ex)
                        {
                            addquery(ex.ToString());
                        }



                        XmlNodeList nodelist = xmlDoc.SelectNodes("tracking-detail/significant-events/occurrence");
                        addquery("CANADAPOST nodelist.Count:" + nodelist.Count.ToString() + ":: Tracking No :" + TrackingNo);

                        if (nodelist.Count > 0)
                        {
                            addquery("INSIDE CANADAPOST nodelist.Count:" + nodelist.Count.ToString() + ":: Tracking No :" + TrackingNo);

                            string track_podsignature = "";
                            string track_podLocation = "";
                            DateTime? dtPODDateTime = null;
                            string track_PODStatus = "";
                            int k = 0;
                            string statusdate = "", statusdatetime = "";
                            try
                            {
                                statusdatetime = nodelist[k].SelectSingleNode("event-date").InnerText + " " + nodelist[k].SelectSingleNode("event-time").InnerText;
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                dtPODDateTime = Convert.ToDateTime(statusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                            }
                            catch (Exception ex)
                            {

                            }
                            try
                            {
                                track_podsignature = nodelist[k].SelectSingleNode("signatory-name").InnerText;
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                track_podLocation = nodelist[k].SelectSingleNode("event-site").InnerText + ", " + nodelist[k].SelectSingleNode("event-province").InnerText;

                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                track_PODStatus = nodelist[k].SelectSingleNode("event-description").InnerText;
                                if (track_PODStatus == "Delivered" || track_PODStatus == "Signature unavailable; verbal signature.")
                                {
                                    track_PODStatus = "DELIVERED";
                                }
                            }
                            catch (Exception ex)
                            {
                                track_PODStatus = "";
                            }

                            string strStatus_code = "";
                            string pstrInsertQuery = "";
                            try
                            {

                                if (track_PODStatus == "DELIVERED")
                                {
                                    strStatus_code = "DEL";
                                    deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);// ===== Deleting the existed record.

                                    System.Threading.Thread.Sleep(100);
                                    if (nodelist.Count > 0)
                                    {
                                        pstrInsertQuery = "insert into[XCARRIER_SHIPPING_VISIBILITY] (shipping_num, POD_DATETIME, POD_SIGN, POD_STATUS, Tracking_num, StatusCode, CURRENT_PLACE, CARRIER) values ";
                                    }



                                    for (int i = 0; i < nodelist.Count; i++)
                                    {
                                        string ptrack_podLocation = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrStatus_code = "", ptrack_podsignature = "";
                                        DateTime? pdtPODDateTime = null;

                                        try
                                        {
                                            pstatusdatetime = nodelist[i].SelectSingleNode("event-date").InnerText + " " + nodelist[i].SelectSingleNode("event-time").InnerText;
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            pdtPODDateTime = Convert.ToDateTime(pstatusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            ptrack_PODStatus = nodelist[i].SelectSingleNode("event-description").InnerText;
                                            if (ptrack_PODStatus == "Delivered" || ptrack_PODStatus == "Signature unavailable; verbal signature.")
                                            {
                                                ptrack_PODStatus = "Delivered";
                                                pstrStatus_code = "DEL";
                                            }
                                            else
                                            {
                                                // ptrack_PODStatus = "In-Transit";
                                                pstrStatus_code = "INT";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ptrack_PODStatus = "";
                                        }

                                        string pstrDatetime = null;
                                        try
                                        {
                                            pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                        }
                                        catch (Exception ex)
                                        {


                                        }
                                        try
                                        {
                                            ptrack_podsignature = nodelist[i].SelectSingleNode("signatory-name").InnerText;

                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            ptrack_podLocation = nodelist[i].SelectSingleNode("event-site").InnerText + ", " + nodelist[i].SelectSingleNode("event-province").InnerText;

                                        }
                                        catch (Exception ex)
                                        {

                                        }


                                        pstrInsertQuery = pstrInsertQuery + "('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'),";

                                    }
                                    if (pstrInsertQuery != "")
                                    {
                                        try
                                        {
                                            pstrInsertQuery = pstrInsertQuery.Substring(0, pstrInsertQuery.Length - 1);
                                            addquery("CANADAPOST pstrInsertQuery :: " + pstrInsertQuery + ":: TrackingNumber" + TrackingNo);
                                            SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                            if (con.State == ConnectionState.Closed)
                                            {
                                                con.Open();
                                            }
                                            SqlCmd.ExecuteNonQuery();

                                            if (con.State == ConnectionState.Open)
                                            {
                                                con.Close();
                                            }

                                            System.Threading.Thread.Sleep(100);
                                        }
                                        catch (Exception ex)
                                        {
                                            addquery("CANADAPOST Ship Visibility Insert Exception :: TrackingNumber" + TrackingNo + " ::" + ex.ToString());

                                            if (con.State == ConnectionState.Open)
                                                con.Close();

                                        }
                                    }
                                }
                                else
                                {
                                    strStatus_code = "INT";
                                }

                            }
                            catch (Exception ex)
                            {
                                addquery("");
                                // strStatus_code = "";
                            }


                            XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);


                            if (ERPUpdateFlag.ToUpper() == "TRUE" && FeederSystemName != "Manual" && k.ToString() == "0")
                            {
                                ERPUPDATE(erpenginepath, PlantID, CustomerID, TrackingNo, track_PODStatus, track_podsignature, dtPODDateTime, ERPName, DeliveryNUM, carrier, shippingno);
                            }

                        }
                    }



                }
                catch (Exception ex)
                {
                    addquery("Unable run CANADAPOST_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
                }

            }
            catch (Exception ex)
            {
                addquery("Unable run CANADAPOST_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
            }
        }




        public static void getSPODLetterRequest(string AccountNo, string servicetype, string trackingno, string sFedExMeterNo, string sFedExCspKey, string sFedExCspPwd, string sFedExUserKey, string sFedExUserPwd, string strpath)
        {
            StreamReader reader = null;
            string stripresponse = "";
            XmlDocument MyXMLDocument = new XmlDocument();
            string strXML = "";
            try
            {
                strXML = strXML + "<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/'>";
                strXML = strXML + "<soapenv:Body>";
                strXML = strXML + "<SignatureProofOfDeliveryLetterRequest xmlns='http://fedex.com/ws/track/v6'>";
                strXML = strXML + "<WebAuthenticationDetail>";
                strXML = strXML + "<CspCredential>";
                strXML = strXML + "<Key>" + sFedExCspKey + "</Key>";
                strXML = strXML + "<Password>" + sFedExCspPwd + "</Password>";
                strXML = strXML + "</CspCredential>";
                strXML = strXML + "<UserCredential>";
                strXML = strXML + "<Key>" + sFedExUserKey + "</Key>";
                strXML = strXML + "<Password>" + sFedExUserPwd + "</Password>";
                strXML = strXML + "</UserCredential>";
                strXML = strXML + "</WebAuthenticationDetail>";
                strXML = strXML + "<ClientDetail>";
                strXML = strXML + "<AccountNumber>" + AccountNo + "</AccountNumber>";
                strXML = strXML + "<MeterNumber>" + sFedExMeterNo + "</MeterNumber>";
                strXML = strXML + "</ClientDetail>";
                strXML = strXML + "<TransactionDetail>";
                strXML = strXML + "<CustomerTransactionId>**SPOD REQUEST**</CustomerTransactionId>";
                strXML = strXML + "</TransactionDetail>";
                strXML = strXML + "<Version>";
                strXML = strXML + "<ServiceId>trck</ServiceId>";
                strXML = strXML + "<Major>6</Major>";
                strXML = strXML + "<Intermediate>0</Intermediate>";
                strXML = strXML + "<Minor>0</Minor>";
                strXML = strXML + "</Version>";
                strXML = strXML + "<QualifiedTrackingNumber>";
                strXML = strXML + "<TrackingNumber>" + trackingno + "</TrackingNumber>";
                strXML = strXML + "</QualifiedTrackingNumber>";
                strXML = strXML + "<LetterFormat>PDF</LetterFormat>";
                strXML = strXML + "</SignatureProofOfDeliveryLetterRequest>";
                strXML = strXML + "</soapenv:Body>";
                strXML = strXML + "</soapenv:Envelope>";
                try
                {
                    string mydocpath1 = LogPath; //"C:\\Processweaver\\BackUp";


                    using (System.IO.StreamWriter outfile =
                      new System.IO.StreamWriter(mydocpath1 + @"\FedExSignaturePODRequest.xml"))
                    {
                        outfile.Write(strXML);
                        outfile.Close();
                        outfile.Dispose();
                        //GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest myRequest1 = (HttpWebRequest)HttpWebRequest.Create("https://ws.fedex.com/web-services/track");
                try
                {
                    myRequest1.AllowAutoRedirect = false;
                    myRequest1.Method = "POST";
                    myRequest1.ContentType = "text/xml";
                    //"application/x-www-form-urlencoded"
                    //Create post stream
                    Stream RequestStream1 = myRequest1.GetRequestStream();
                    byte[] SomeBytes1 = Encoding.UTF8.GetBytes(strXML);
                    RequestStream1.Write(SomeBytes1, 0, SomeBytes1.Length);
                    RequestStream1.Close();
                    //Send request and get response
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebResponse myResponse = (HttpWebResponse)myRequest1.GetResponse();
                    if (myResponse.StatusCode == HttpStatusCode.OK)
                    {
                        //Get the stream.
                        Stream ReceiveStream = myResponse.GetResponseStream();
                        Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                        //send the stream to a reader. 
                        StreamReader readStream = new StreamReader(ReceiveStream, encode);
                        //Read the result
                        string Result = readStream.ReadToEnd();
                        Result = Result.Replace("soapenv:", "");
                        Result = Result.Replace("env:", "");
                        Result = Result.Replace("v4:", "");
                        Result = Result.Replace("<ns:", "<");
                        Result = Result.Replace("</ns:", "</");

                        try
                        {
                            string mydocpath1 = LogPath; //"C:\\Processweaver\\BackUp";


                            using (System.IO.StreamWriter outfile =
                              new System.IO.StreamWriter(mydocpath1 + @"\FedExSignaturePODResponse.xml"))
                            {
                                outfile.Write(Result);
                                outfile.Close();
                                outfile.Dispose();
                                //GC.Collect();
                            }
                        }
                        catch (Exception ex)
                        {
                        }

                        MyXMLDocument = new System.Xml.XmlDocument();
                        MyXMLDocument.LoadXml(Result);
                        string severiaty = "";
                        XmlDataDocument xmldoc1 = new XmlDataDocument();
                        xmldoc1.LoadXml(Result);
                        //XmlNodeList xmlnode1 = default(XmlNodeList);
                        //xmlnode1 = xmldoc1.GetElementsByTagName("Severity");
                        severiaty = xmldoc1.GetElementsByTagName("Severity")[0].InnerText;
                        Console.WriteLine("FedEx POD Status:" + severiaty);
                        if (severiaty == "ERROR" | severiaty == "FAILURE")
                        {

                        }
                        else
                        {
                            string imagedata = xmldoc1.GetElementsByTagName("Letter")[0].InnerText;
                            string FileName = strpath + "\\" + trackingno + ".pdf";
                            //string FileName = strPath + trackingno + ".pdf";
                            //Console.WriteLine("FedEx POD File :---" + FileName);
                            // Dim imagedata As String = ""
                            byte[] base64byte = Convert.FromBase64String(imagedata);
                            FileStream obj = File.Create(FileName);
                            obj.Write(base64byte, 0, base64byte.Length);
                            obj.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    //System.Console.WriteLine("INSIDE FedEx POD EXCEPTION: " + ex.Message);

                }
            }
            catch (Exception ex)
            {
                addquery("INSIDE getSPODLetter EXCEPTION: " + ex.Message);

            }
        }
        // public static void UPS_FREIGHT_Status(string AccountNo, string TrackingNo, string PlantID, string Service, string connectionstring, String strShipFromCountry,
        // String strDate, string shippingno, string erpenginepath, string ERPUpdateFlag, string LabelPath, DataSet dsCarrier, SqlConnection con, string CustomerID,
        //   string carrier, string ERPName, string DeliveryNUM, string FeederSystemName, string MastertrackingNo)
        public static void UPS_FREIGHT_Status(object pparam_obj)
        {
            string shippingno = "", ERPUpdateFlag = "", TrackingNo = "", ERPName = "", FeederSystemName = "", strShipFromCountry = "", Service = "", MastertrackingNo = "", DeliveryNUM = "", CustomerID = "", strDate = "", AccountNo = "", carrier = "", PlantID = "", connectionstring = "", erpenginepath = "", LabelPath = "";
            SqlConnection con = null;
            try
            {
                Array UPSFREIGHTargArray = new object[2];
                UPSFREIGHTargArray = (Array)pparam_obj;

                AccountNo = (string)UPSFREIGHTargArray.GetValue(0);
                TrackingNo = (string)UPSFREIGHTargArray.GetValue(1);
                PlantID = (string)UPSFREIGHTargArray.GetValue(2);
                Service = (string)UPSFREIGHTargArray.GetValue(3);
                connectionstring = (string)UPSFREIGHTargArray.GetValue(4);
                strShipFromCountry = (string)UPSFREIGHTargArray.GetValue(5);
                strDate = (string)UPSFREIGHTargArray.GetValue(6);
                shippingno = (string)UPSFREIGHTargArray.GetValue(7);
                erpenginepath = (string)UPSFREIGHTargArray.GetValue(8);
                ERPUpdateFlag = (string)UPSFREIGHTargArray.GetValue(9);
                LabelPath = (string)UPSFREIGHTargArray.GetValue(10);
                DataSet dsCarrier = (DataSet)UPSFREIGHTargArray.GetValue(11);
                con = (SqlConnection)UPSFREIGHTargArray.GetValue(12);
                CustomerID = (string)UPSFREIGHTargArray.GetValue(13);
                carrier = (string)UPSFREIGHTargArray.GetValue(14);
                ERPName = (string)UPSFREIGHTargArray.GetValue(15);
                DeliveryNUM = (string)UPSFREIGHTargArray.GetValue(16);
                FeederSystemName = (string)UPSFREIGHTargArray.GetValue(17);
                MastertrackingNo = (string)UPSFREIGHTargArray.GetValue(18);

                DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number

                string LicenseNo = "";
                string UserId = "";
                string Password = "";
                string _accountno = "";
                string custid = "";

                if (dataRows.Length > 0)
                {
                    LicenseNo = dataRows[0]["LICENSE_NUMBER"].ToString();
                    UserId = dataRows[0]["User_ID"].ToString();
                    Password = dataRows[0]["Password"].ToString();
                    _accountno = dataRows[0]["Account_Number"].ToString();
                    custid = dataRows[0]["COMPANY_ID"].ToString();

                }



                DateTime dtShipDate = DateTime.Now;
                string Upsrequest = "";
                string status = "";
                Upsrequest = "";
                Upsrequest = Upsrequest + "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:v1=\"http://www.ups.com/XMLSchema/XOLTWS/UPSS/v1.0\" xmlns:v2=\"http://www.ups.com/XMLSchema/XOLTWS/Track/v2.0\" xmlns:v11=\"http://www.ups.com/XMLSchema/XOLTWS/Common/v1.0\">" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<soapenv:Header>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v1:UPSSecurity>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v1:UsernameToken>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v1:Username>" + UserId + "</v1:Username>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v1:Password>" + Password + "</v1:Password>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</v1:UsernameToken>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v1:ServiceAccessToken>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v1:AccessLicenseNumber>" + LicenseNo + "</v1:AccessLicenseNumber>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</v1:ServiceAccessToken>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</v1:UPSSecurity>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</soapenv:Header>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<soapenv:Body>";
                Upsrequest = Upsrequest + "<v2:TrackRequest>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v11:Request>";
                Upsrequest = Upsrequest + "<v11:RequestOption>15</v11:RequestOption>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v11:SubVersion>1801</v11:SubVersion>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v11:TransactionReference>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v11:CustomerContext>Your Test Case Summary Description</v11:CustomerContext>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</v11:TransactionReference>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</v11:Request>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "<v2:InquiryNumber>" + TrackingNo + "</v2:InquiryNumber>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</v2:TrackRequest>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</soapenv:Body>" + System.Environment.NewLine;
                Upsrequest = Upsrequest + "</soapenv:Envelope>";

                try
                {
                    using (System.IO.StreamWriter outfile =
                    new System.IO.StreamWriter(LogPath + @"\UPS_FREIGHT_TrackRequest.xml"))
                    {
                        outfile.Write(Upsrequest);
                        outfile.Close();
                        outfile.Dispose();
                        //GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                    addquery(ex.ToString());
                }

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://onlinetools.ups.com/webservices/Track");

                request.Method = "POST";
                request.AllowAutoRedirect = false;

                byte[] bytes = Encoding.UTF8.GetBytes(Upsrequest);
                request.ContentLength = bytes.Length;
                request.ContentType = "text/xml;charset=UTF-8";

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                    requestStream.Dispose();

                }

                XmlDocument xmlDoc = new XmlDocument();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse();
                if (myResponse.StatusCode == HttpStatusCode.OK)
                {
                    Stream ReceiveStream = myResponse.GetResponseStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader readStream = new StreamReader(ReceiveStream, encode);
                    string Result = readStream.ReadToEnd();
                    readStream.Close();
                    readStream.Dispose();

                    Result = Result.Replace("soapenv:", "");
                    Result = Result.Replace("common:", "");
                    Result = Result.Replace("trk:", "");
                    Result = Result.Replace("err:", "");
                    xmlDoc.LoadXml(Result.Trim());
                    try
                    {
                        using (System.IO.StreamWriter outfile =
                        new System.IO.StreamWriter(LogPath + @"\UPS_FREIGHT_TrackResponse.xml"))
                        {
                            outfile.Write(Result);
                            outfile.Close();
                            outfile.Dispose();
                            //GC.Collect();
                        }
                    }
                    catch (Exception ex)
                    {
                        addquery(ex.ToString());
                    }


                    XmlNodeList nodelist = xmlDoc.SelectNodes("/Envelope/Body/TrackResponse/Shipment/Activity");
                    if (nodelist.Count > 0)
                    {

                        string track_podsignature = "";
                        string track_podLocation = "";
                        DateTime? dtPODDateTime = null;
                        string track_PODStatus = "";

                        int k = 0;
                        string statusdate = "", statusdatetime = "";
                        try
                        {
                            statusdate = nodelist[k].SelectSingleNode("Date").InnerText;
                            string year = statusdate.Substring(0, 4);
                            string month = statusdate.Substring(4, 2);
                            string day = statusdate.Substring(6, 2);
                            statusdate = year + "-" + month + "-" + day;

                            statusdatetime = nodelist[k].SelectSingleNode("Time").InnerText;
                            string hour = statusdatetime.Substring(0, 2);
                            string minute = statusdatetime.Substring(2, 2);
                            string sec = statusdatetime.Substring(4, 2);
                            statusdatetime = hour + ":" + minute + ":" + sec;
                        }
                        catch (Exception ex) { }

                        try
                        {
                            dtPODDateTime = DateTime.ParseExact(statusdate + " " + statusdatetime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                        }
                        catch (Exception ex) { }
                        try
                        {
                            track_podsignature = xmlDoc.GetElementsByTagName("SignedForByName")[0].InnerText;
                        }
                        catch (Exception exp) { }
                        try
                        {
                            track_podLocation = nodelist[k].SelectSingleNode("ActivityLocation/City").InnerText + ", " + nodelist[k].SelectSingleNode("ActivityLocation/StateProvinceCode").InnerText;
                        }
                        catch (Exception exp) { }
                        try
                        {
                            track_PODStatus = nodelist[k].SelectSingleNode("/Envelope/Body/TrackResponse/Shipment/CurrentStatus/Description").InnerText;
                            if (track_PODStatus == "Delivered")
                            {
                                track_PODStatus = "DELIVERED";
                            }
                        }
                        catch (Exception ex) { track_PODStatus = ""; }
                        string strStatus_code = "";
                        try
                        {
                            if (track_PODStatus == "DELIVERED")
                            {
                                strStatus_code = "DEL";
                                deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);

                                System.Threading.Thread.Sleep(100);

                                for (int i = 0; i < nodelist.Count; i++)
                                {
                                    string pstatusdate = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrInsertQuery = "", pstrStatus_code = "", ptrack_podLocation = "", ptrack_podsignature = "";
                                    DateTime? pdtPODDateTime = null;

                                    try
                                    {
                                        pstatusdate = nodelist[i].SelectSingleNode("Date").InnerText;
                                        string year = pstatusdate.Substring(0, 4);
                                        string month = pstatusdate.Substring(4, 2);
                                        string day = pstatusdate.Substring(6, 2);
                                        pstatusdate = year + "-" + month + "-" + day;

                                        pstatusdatetime = nodelist[i].SelectSingleNode("Time").InnerText;
                                        string hour = pstatusdatetime.Substring(0, 2);
                                        string minute = pstatusdatetime.Substring(2, 2);
                                        string sec = pstatusdatetime.Substring(4, 2);
                                        pstatusdatetime = hour + ":" + minute + ":" + sec;

                                    }
                                    catch (Exception ex) { }

                                    try
                                    {
                                        pdtPODDateTime = DateTime.ParseExact(pstatusdate + " " + pstatusdatetime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                    }
                                    catch (Exception ex) { }
                                    try
                                    {
                                        //ptrack_PODStatus = nodelist[k].SelectSingleNode("/Envelope/Body/TrackResponse/Shipment/CurrentStatus/Description").InnerText;
                                        ptrack_PODStatus = nodelist[i].SelectSingleNode("Description").InnerText;

                                        if (ptrack_PODStatus == "Shipment has been delivered to consignee")
                                        {
                                            ptrack_PODStatus = "Delivered";
                                            pstrStatus_code = "DEL";
                                        }
                                        else { pstrStatus_code = "INT"; }
                                    }
                                    catch (Exception ex) { ptrack_PODStatus = ""; }

                                    string pstrDatetime = null;
                                    try
                                    {
                                        pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                    }
                                    catch (Exception ex) { }
                                    try
                                    {

                                        ptrack_podLocation = nodelist[i].SelectSingleNode("ActivityLocation/City").InnerText + ", " + nodelist[i].SelectSingleNode("ActivityLocation/StateProvinceCode").InnerText;
                                        ptrack_podsignature = xmlDoc.GetElementsByTagName("SignedForByName")[0].InnerText;
                                    }
                                    catch (Exception exp) { }

                                    pstrInsertQuery = "insert into [XCARRIER_SHIPPING_VISIBILITY] (shipping_num,POD_DATETIME,POD_SIGN,POD_STATUS,Tracking_num,StatusCode,CURRENT_PLACE,CARRIER) values('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'); ";

                                    if (pstrInsertQuery != "")
                                    {
                                        try
                                        {

                                            SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                            if (con.State == ConnectionState.Closed)
                                            {
                                                con.Open();
                                            }
                                            SqlCmd.ExecuteNonQuery();
                                            if (con.State == ConnectionState.Open)
                                            {
                                                con.Close();
                                            }

                                            System.Threading.Thread.Sleep(100);

                                        }
                                        catch (Exception ex)
                                        {
                                            if (con.State == ConnectionState.Open)
                                                con.Close();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                strStatus_code = "INT";
                            }

                        }
                        catch (Exception ex)
                        {// strStatus_code = ""; }



                        }
                        XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);

                        if (ERPUpdateFlag.ToUpper() == "TRUE" && FeederSystemName != "Manual" && k.ToString() == "0")
                        {
                            ERPUPDATE(erpenginepath, PlantID, CustomerID, TrackingNo, track_PODStatus, track_podsignature, dtPODDateTime, ERPName, DeliveryNUM, carrier, shippingno);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                addquery("Unable run UPS_FREIGHT_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
            }

        }
        public static void ESTES_Status(object pparam_obj)
        {
            string TrackingNo = "";
            string shippingno = "";
            DataSet ESTESDS = new DataSet();
            string ESTESUserid = "", ESTESPassword = "", AuthHeader = "";
           

            try
            {
                Array argArray = new object[2];
                argArray = (Array)pparam_obj;

                string AccountNo = (string)argArray.GetValue(0);
                TrackingNo = (string)argArray.GetValue(1);
                string PlantID = (string)argArray.GetValue(2);
                string Service = (string)argArray.GetValue(3);
                string connectionstring = (string)argArray.GetValue(4);
                string strShipFromCountry = (string)argArray.GetValue(5);
                string strDate = (string)argArray.GetValue(6);
                shippingno = (string)argArray.GetValue(7);
                string erpenginepath = (string)argArray.GetValue(8);
                string ERPUpdateFlag = (string)argArray.GetValue(9);
                string LabelPath = (string)argArray.GetValue(10);
                DataSet dsCarrier = (DataSet)argArray.GetValue(11);
                SqlConnection con = (SqlConnection)argArray.GetValue(12);
                string CustomerID = (string)argArray.GetValue(13);
                string carrier = (string)argArray.GetValue(14);
                string ERPName = (string)argArray.GetValue(15);
                string DeliveryNUM = (string)argArray.GetValue(16);
                string FeederSystemName = (string)argArray.GetValue(17);
                string MastertrackingNo = (string)argArray.GetValue(18);

                DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number


                if (dataRows.Length > 0)
                {
                    ESTESUserid = dataRows[0]["USER_ID"].ToString();
                    ESTESPassword = dataRows[0]["PASSWORD"].ToString();

                }

                string request = "";

                request += "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:v1=\"https://api.estes-express.com/ws/tools/shipment/tracking/v1.1/\">" + Environment.NewLine;
                request += "<soapenv:Header/>" + Environment.NewLine;
                request += "<soapenv:Body>" + Environment.NewLine;
                request += "<v1:shipmentTracking>" + Environment.NewLine;
                request += "<search>" + Environment.NewLine;
                request += "<requestID/>" + Environment.NewLine;
                request += "<!--Optional:-->" + Environment.NewLine;
                request += "<pro>" + TrackingNo + "</pro>" + Environment.NewLine;
                request += "<!--Optional:-->" + Environment.NewLine;
                request += "<bol></bol>" + Environment.NewLine;
                request += "</search>" + Environment.NewLine;
                request += "</v1:shipmentTracking>" + Environment.NewLine;
                request += "</soapenv:Body>" + Environment.NewLine;
                request += "</soapenv:Envelope>" + Environment.NewLine;


                try
                {

                    try
                    {
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                    }
                    catch (Exception ex)
                    {

                    }

                    AuthHeader = ESTESUserid + ":" + ESTESPassword;
                    byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes(AuthHeader);
                    string encodedAuthHeaderText = Convert.ToBase64String(bytesToEncode);
                    HttpWebRequest myRequestESTES = (HttpWebRequest)HttpWebRequest.Create("https://api.estes-express.com:443/ws/estesrtshipmenttracking.base.ws.provider.soapws:EstesShipmentTracking/estesrtshipmenttracking_base_ws_provider_soapws_EstesShipmentTracking_Port");
                    myRequestESTES.AllowAutoRedirect = false;
                    myRequestESTES.Method = "POST";
                    myRequestESTES.ContentType = "text/xml;charset=UTF-8";
                    // myRequestESTES.Headers.Add("Authorization", encodedAuthHeaderText);
                    myRequestESTES.Headers.Add("Authorization", "Basic " + encodedAuthHeaderText);
                    myRequestESTES.Headers.Add("SOAPAction", "estesrtshipmenttracking_base_ws_provider_soapws_EstesShipmentTracking_Binder_shipmentTracking");

                    myRequestESTES.PreAuthenticate = true;

                    // Create post stream
                    Stream RequestStream = myRequestESTES.GetRequestStream();
                    byte[] SomeBytes1 = Encoding.UTF8.GetBytes(request);
                    RequestStream.Write(SomeBytes1, 0, SomeBytes1.Length);
                    RequestStream.Close();


                    XmlDocument xmlDoc = new XmlDocument();

                    try
                    {
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                    }
                    catch (Exception ex)
                    {
                    }

                    HttpWebResponse myResponse = (HttpWebResponse)myRequestESTES.GetResponse();
                    if (myResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Stream ReceiveStream = myResponse.GetResponseStream();
                        Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

                        StreamReader readStream = new StreamReader(ReceiveStream, encode);

                        string Result = readStream.ReadToEnd();
                        System.Xml.XmlDocument MyXMLDocument;
                        MyXMLDocument = new System.Xml.XmlDocument();
                        string Formatted = RemoveAllNamespaces(Result);
                        MyXMLDocument.LoadXml(Formatted);
                        

                        XmlNodeList nodelist = MyXMLDocument.SelectNodes("Envelope/Body/shipmentTrackingResponse/trackingInfo/shipments/shipments");
                        if (nodelist.Count > 0)
                        {

                            string track_podsignature = "";
                            string track_podLocation = "";
                            DateTime? dtPODDateTime = null;
                            string track_PODStatus = "";
                            int k = 0;
                            string statusdate = "", statusdatetime = "";
                            try
                            {
                                statusdatetime = nodelist[k].SelectSingleNode("eventTimeStamp").InnerText;
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                dtPODDateTime = Convert.ToDateTime(statusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                            }
                            catch (Exception ex)
                            {

                            }
                            try
                            {
                                track_podsignature = "";
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                track_podLocation = "";

                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                track_PODStatus = nodelist[k].SelectSingleNode("status").InnerText;
                                if (track_PODStatus == "Delivered")
                                {
                                    track_PODStatus = "DELIVERED";
                                }
                            }
                            catch (Exception ex)
                            {
                                track_PODStatus = "";
                            }
                            string strStatus_code = "";
                            string pstrInsertQuery = "";
                            try
                            {

                                if (track_PODStatus == "DELIVERED")
                                {
                                    strStatus_code = "DEL";
                                    deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);

                                    System.Threading.Thread.Sleep(100);

                                    if (nodelist.Count > 0)
                                    {
                                        pstrInsertQuery = "insert into [XCARRIER_SHIPPING_VISIBILITY] (shipping_num,POD_DATETIME,POD_SIGN,POD_STATUS,Tracking_num,StatusCode,CURRENT_PLACE,CARRIER) values ";
                                    }


                                    for (int i = 0; i < nodelist.Count; i++)
                                    {
                                        string ptrack_podLocation = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrStatus_code = "", ptrack_podsignature = "";
                                        DateTime? pdtPODDateTime = null;

                                        try
                                        {
                                            pstatusdatetime = nodelist[k].SelectSingleNode("eventTimeStamp").InnerText;
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            pdtPODDateTime = Convert.ToDateTime(pstatusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            ptrack_PODStatus = nodelist[i].SelectSingleNode("status").InnerText;
                                            if (ptrack_PODStatus == "Delivered")
                                            {
                                                ptrack_PODStatus = "Delivered";
                                                pstrStatus_code = "DEL";
                                            }
                                            else
                                            {
                                                // ptrack_PODStatus = "In-Transit";
                                                pstrStatus_code = "INT";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ptrack_PODStatus = "";
                                        }

                                        string pstrDatetime = null;
                                        try
                                        {
                                            pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                        }
                                        catch (Exception ex)
                                        {


                                        }
                                        try
                                        {
                                            ptrack_podsignature = "";

                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            ptrack_podLocation = "";

                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        pstrInsertQuery = pstrInsertQuery + "('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'),";


                                    }
                                    if (pstrInsertQuery != "")
                                    {
                                        try
                                        {
                                            pstrInsertQuery = pstrInsertQuery.Substring(0, pstrInsertQuery.Length - 1);
                                            addquery("ESTES pstrInsertQuery :: " + pstrInsertQuery + ":: TrackingNumber" + TrackingNo);

                                            SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                            if (con.State == ConnectionState.Closed)
                                            {
                                                con.Open();
                                            }
                                            SqlCmd.ExecuteNonQuery();
                                            if (con.State == ConnectionState.Open)
                                            {
                                                con.Close();
                                            }

                                            System.Threading.Thread.Sleep(100);

                                        }
                                        catch (Exception ex)
                                        {
                                            addquery("ESTES Ship Visibility Insert Exception :: TrackingNumber" + TrackingNo + " ::" + ex.ToString());

                                            if (con.State == ConnectionState.Open)
                                                con.Close();
                                        }
                                    }
                                }
                                else
                                {
                                    strStatus_code = "INT";
                                }

                            }
                            catch (Exception ex)
                            {
                                // strStatus_code = "";
                            }

                            XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);

                        }
                    }



                }
                catch (Exception ex)
                {
                    addquery("Unable run ESTES for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
                }

            }
            catch (Exception ex)
            {
                addquery("Unable run ESTES for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
            }
        }
        public static string RemoveAllNamespaces(string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));

            return xmlDocumentWithoutNs.ToString();
        }
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }



        public static void SEFL_Status(object pparam_obj)
        {
            string TrackingNo = "";
            string shippingno = "";

            try
            {
                Array argArray = new object[2];
                argArray = (Array)pparam_obj;

                string AccountNo = (string)argArray.GetValue(0);
                TrackingNo = (string)argArray.GetValue(1);
                string PlantID = (string)argArray.GetValue(2);
                string Service = (string)argArray.GetValue(3);
                string connectionstring = (string)argArray.GetValue(4);
                string strShipFromCountry = (string)argArray.GetValue(5);
                string strDate = (string)argArray.GetValue(6);
                shippingno = (string)argArray.GetValue(7);
                string erpenginepath = (string)argArray.GetValue(8);
                string ERPUpdateFlag = (string)argArray.GetValue(9);
                string LabelPath = (string)argArray.GetValue(10);
                DataSet dsCarrier = (DataSet)argArray.GetValue(11);
                SqlConnection con = (SqlConnection)argArray.GetValue(12);
                string CustomerID = (string)argArray.GetValue(13);
                string carrier = (string)argArray.GetValue(14);
                string ERPName = (string)argArray.GetValue(15);
                string DeliveryNUM = (string)argArray.GetValue(16);
                string FeederSystemName = (string)argArray.GetValue(17);
                string MastertrackingNo = (string)argArray.GetValue(18);
                


                string URL = "";


                if (string.IsNullOrEmpty(URL))
                {
                    URL = "https://www.sefl.com/webconnect/tracing?Type=PN&RefNum1=" + TrackingNo + "&output=XML";
                }


                try
                {
                    using (System.IO.StreamWriter outfile =
                    new System.IO.StreamWriter(LogPath + @"\SEFL_TrackRequest.txt"))
                    {
                        outfile.Write(URL);
                        outfile.Close();
                        outfile.Dispose();
                        //GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                    addquery(ex.ToString());
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL);
                request.AllowAutoRedirect = false;

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                
                XmlDocument xmlDoc = new XmlDocument();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse();
                if (myResponse.StatusCode == HttpStatusCode.OK)
                {
                    Stream ReceiveStream = myResponse.GetResponseStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader readStream = new StreamReader(ReceiveStream, encode);
                    string Result = readStream.ReadToEnd();

                    readStream.Close();
                    readStream.Dispose();

                    encode = null;

                    ReceiveStream.Close();
                    ReceiveStream.Dispose();

                    myResponse.Close();
                    myResponse.Dispose();
                    
                    xmlDoc.LoadXml(Result.Trim());

                    string Formatted = RemoveAllNamespaces(Result);
                    xmlDoc.LoadXml(Formatted);

                    try
                    {
                        using (System.IO.StreamWriter outfile =
                        new System.IO.StreamWriter(LogPath + @"\SEFL_TrackResponse.xml"))
                        {
                            outfile.Write(Result);
                            outfile.Close();
                            outfile.Dispose();
                            //GC.Collect();
                        }
                    }
                    catch (Exception ex)
                    {
                        addquery(ex.ToString());
                    }


                    XmlNodeList nodelist = xmlDoc.SelectNodes("root/pro/detailInformation/detail");
                    addquery("SEFL nodelist.Count:" + nodelist.Count.ToString() + ":: Tracking No :" + TrackingNo);

                    if (nodelist.Count > 0)
                    {
                        addquery("INSIDE SEFL nodelist.Count:" + nodelist.Count.ToString() + ":: Tracking No :" + TrackingNo);

                        string track_podsignature = "";
                        string track_podLocation = "";
                        DateTime? dtPODDateTime = null;
                        string track_PODStatus = "";


                        string track_PODCity = "", track_PODState = "";


                        int k = 0;
                        string  statusdatetime = "";
                        try
                        {
                            statusdatetime = nodelist[k].SelectSingleNode("date").InnerText + " " + nodelist[k].SelectSingleNode("time").InnerText.Replace("AM", " ").Replace("PM", " ");

                        }
                        catch (Exception ex)
                        {

                        }

                        try
                        {
                            dtPODDateTime = Convert.ToDateTime(statusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                        }
                        catch (Exception ex)
                        {

                        }
                        try
                        {
                            track_podLocation = nodelist[k].SelectSingleNode("city").InnerText + ", " + nodelist[k].SelectSingleNode("state").InnerText;
                        }
                        catch (Exception ex)
                        {

                        }

                      
                        try
                        {
                            track_PODCity = nodelist[k].SelectSingleNode("city").InnerText;
                        }
                        catch (Exception ex)
                        {

                        }
                        try
                        {
                            track_PODState = nodelist[k].SelectSingleNode("state").InnerText;
                        }
                        catch (Exception ex)
                        {

                        }

                        try
                        {
                            track_PODStatus = xmlDoc.SelectSingleNode("root/pro/currentStatusDescription").InnerText;
                            if (track_PODStatus=="Delivered")
                            {
                                track_PODStatus = "DELIVERED";
                            }
                        }
                        catch (Exception ex)
                        {
                            track_PODStatus = "";
                        }




                        string strStatus_code = "";
                        string pstrInsertQuery = "";
                        try
                        {

                            if (track_PODStatus == "DELIVERED")
                            {
                                strStatus_code = "DEL";
                                deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);

                                System.Threading.Thread.Sleep(100);
                                if (nodelist.Count > 0)
                                {
                                    pstrInsertQuery = "insert into[XCARRIER_SHIPPING_VISIBILITY] (shipping_num, POD_DATETIME, POD_SIGN, POD_STATUS, Tracking_num, StatusCode, CURRENT_PLACE, CARRIER) values ";
                                }



                                for (int i = 0; i < nodelist.Count; i++)
                                {
                                    string ptrack_podLocation = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrStatus_code = "", ptrack_podsignature = "";

                                    DateTime? pdtPODDateTime = null;

                                    try
                                    {
                                        pstatusdatetime = nodelist[i].SelectSingleNode("date").InnerText +" "+ nodelist[i].SelectSingleNode("time").InnerText.Replace("AM" ," ").Replace("PM"," ");
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    try
                                    {
                                        pdtPODDateTime = Convert.ToDateTime(pstatusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    try
                                    {
                                        ptrack_PODStatus = nodelist[i].SelectSingleNode("message").InnerText;
                                        if (track_PODStatus.Contains("Delivered") && nodelist[k].SelectSingleNode("statusCode").InnerText == "DLV")
                                        {
                                            track_PODStatus = "DELIVERED";
                                        }
                                        else
                                        {
                                            // ptrack_PODStatus = "In-Transit";
                                            pstrStatus_code = "INT";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ptrack_PODStatus = "";
                                    }

                                    string pstrDatetime = null;
                                    try
                                    {
                                        pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                    }
                                    catch (Exception ex)
                                    {


                                    }
                                    try
                                    {
                                        ptrack_podsignature = xmlDoc.SelectSingleNode("root/pro/signedBy").InnerText;

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    try
                                    {
                                        ptrack_podLocation = nodelist[i].SelectSingleNode("city").InnerText + ", " + nodelist[i].SelectSingleNode("state").InnerText;

                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    pstrInsertQuery = pstrInsertQuery + "('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'),";

                                }
                                if (pstrInsertQuery != "")
                                {
                                    try
                                    {
                                        pstrInsertQuery = pstrInsertQuery.Substring(0, pstrInsertQuery.Length - 1);
                                        addquery("SEFL pstrInsertQuery :: " + pstrInsertQuery + ":: TrackingNumber" + TrackingNo);
                                        SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                        if (con.State == ConnectionState.Closed)
                                        {
                                            con.Open();
                                        }
                                        SqlCmd.ExecuteNonQuery();

                                        if (con.State == ConnectionState.Open)
                                        {
                                            con.Close();
                                        }

                                        System.Threading.Thread.Sleep(100);
                                    }
                                    catch (Exception ex)
                                    {
                                        addquery("SEFL Ship Visibility Insert Exception :: TrackingNumber" + TrackingNo + " ::" + ex.ToString());

                                        if (con.State == ConnectionState.Open)
                                            con.Close();

                                    }
                                }
                            }
                            else
                            {
                                strStatus_code = "INT";
                            }

                        }
                        catch (Exception ex)
                        {
                            addquery("");
                            // strStatus_code = "";
                        }

                        XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);



                    }
                }



            }
            catch (Exception ex)
            {
                addquery("Unable run FEDEX_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
            }

        }

        public static void SAIA_Status(object pparam_obj)
        {
            string TrackingNo = "";
            string shippingno = "";

            try
            {
                Array argArray = new object[2];
                argArray = (Array)pparam_obj;

                string AccountNo = (string)argArray.GetValue(0);
                TrackingNo = (string)argArray.GetValue(1);
                string PlantID = (string)argArray.GetValue(2);
                string Service = (string)argArray.GetValue(3);
                string connectionstring = (string)argArray.GetValue(4);
                string strShipFromCountry = (string)argArray.GetValue(5);
                string strDate = (string)argArray.GetValue(6);
                shippingno = (string)argArray.GetValue(7);
                string erpenginepath = (string)argArray.GetValue(8);
                string ERPUpdateFlag = (string)argArray.GetValue(9);
                string LabelPath = (string)argArray.GetValue(10);
                DataSet dsCarrier = (DataSet)argArray.GetValue(11);
                SqlConnection con = (SqlConnection)argArray.GetValue(12);
                string CustomerID = (string)argArray.GetValue(13);
                string carrier = (string)argArray.GetValue(14);
                string ERPName = (string)argArray.GetValue(15);
                string DeliveryNUM = (string)argArray.GetValue(16);
                string FeederSystemName = (string)argArray.GetValue(17);
                string MastertrackingNo = (string)argArray.GetValue(18);
               // string OrderTypeName = (string)argArray.GetValue(19);

                


                DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number

                
                string saiauserid = "";
                string saiapwd = "";
                if (dataRows.Length > 0)
                {

                    saiauserid = dataRows[0]["USER_ID"].ToString();
                    saiapwd = dataRows[0]["PASSWORD"].ToString();

                }

                string strXML = "";
                strXML += "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ship=\"http://www.SaiaSecure.com/WebService/Shipment\">" + Environment.NewLine;
                strXML += " <soapenv:Header/>" + Environment.NewLine;
                strXML += " <soapenv:Body>" + Environment.NewLine;
                strXML += " <ship:GetByProNumber>" + Environment.NewLine;
                strXML += " <ship:request>" + Environment.NewLine;
                strXML += " <ship:UserID>" + saiauserid + "</ship:UserID>" + Environment.NewLine; // CENTRALP
                strXML += " <ship:Password>" + saiapwd + "</ship:Password>" + Environment.NewLine; // SHAWNEE
                strXML += " <ship:TestMode>N</ship:TestMode>" + Environment.NewLine; // Y
                strXML += " <ship:ProNumber>"+TrackingNo+"</ship:ProNumber>" + Environment.NewLine; // 124578
                strXML += " </ship:request>" + Environment.NewLine;
                strXML += " </ship:GetByProNumber>" + Environment.NewLine;
                strXML += " </soapenv:Body>" + Environment.NewLine;
                strXML += "</soapenv:Envelope>" + Environment.NewLine;

                try
                {
                    using (System.IO.StreamWriter outfile =
                    new System.IO.StreamWriter(LogPath + @"\SAIA_Request.xml"))
                    {
                        outfile.Write(strXML);
                        outfile.Close();
                        outfile.Dispose();
                        //GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                    addquery(ex.ToString());
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.saiasecure.com/webservice/shipment/soap.asmx");
                request.AllowAutoRedirect = false;
                request.Method = "POST";
                request.ContentType = "text/xml;charset=UTF-8";
                request.Headers.Add("SOAPAction", "http://www.SaiaSecure.com/WebService/Shipment/GetByProNumber");

                request.PreAuthenticate = true;
                request.Timeout = 999999999;
               
                byte[] bytes = Encoding.UTF8.GetBytes(strXML);
                request.ContentLength = bytes.Length;
                request.ContentType = "text/xml;charset=UTF-8";

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                    requestStream.Dispose();

                }

                XmlDocument xmlDoc = new XmlDocument();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse();
                if (myResponse.StatusCode == HttpStatusCode.OK)
                {
                    Stream ReceiveStream = myResponse.GetResponseStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader readStream = new StreamReader(ReceiveStream, encode);
                    string Result = readStream.ReadToEnd();

                    readStream.Close();
                    readStream.Dispose();

                    encode = null;

                    ReceiveStream.Close();
                    ReceiveStream.Dispose();

                    myResponse.Close();
                    myResponse.Dispose();
                    bytes = null;

                    request.Abort();

                    string Formatted = RemoveAllNamespaces(Result);
                    xmlDoc.LoadXml(Formatted);

                   
                    try
                    {
                        using (System.IO.StreamWriter outfile =
                        new System.IO.StreamWriter(LogPath + @"\SAIA_TrackResponse.xml"))
                        {
                            outfile.Write(Result);
                            outfile.Close();
                            outfile.Dispose();
                            //GC.Collect();
                        }
                    }
                    catch (Exception ex)
                    {
                        addquery(ex.ToString());
                    }


                    XmlNodeList nodelist = xmlDoc.SelectNodes("Envelope/Body/GetByProNumberResponse/GetByProNumberResult/History/HistoryItem");
                    addquery("SAIA nodelist.Count:" + nodelist.Count.ToString() + ":: Tracking No :" + TrackingNo);

                    if (nodelist.Count > 0)
                    {
                        addquery("INSIDE SAIA nodelist.Count:" + nodelist.Count.ToString() + ":: Tracking No :" + TrackingNo);

                        string track_podsignature = "";
                        string track_podLocation = "";
                        DateTime? dtPODDateTime = null;
                        string track_PODStatus = "";
                        string track_Notes = "";

                        int k = 0;
                        string  statusdatetime = "";
                        try
                        {
                            statusdatetime = nodelist[k].SelectSingleNode("ActivityDateTime").InnerText;

                        }
                        catch (Exception ex)
                        {

                        }

                        try
                        {
                            dtPODDateTime = Convert.ToDateTime(statusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                        }
                        catch (Exception ex)
                        {

                        }
                        try
                        {
                            track_podsignature = xmlDoc.SelectSingleNode("Envelope/Body/GetByProNumberResponse/GetByProNumberResult/Signature").InnerText;
                        }
                        catch (Exception ex)
                        {

                        }

                        try
                        {
                            track_podLocation = nodelist[k].SelectSingleNode("City").InnerText + ", " + nodelist[k].SelectSingleNode("State").InnerText;

                        }
                        catch (Exception ex)
                        {

                        }
                        
                        try
                        {
                            track_PODStatus = nodelist[k].SelectSingleNode("Activity").InnerText;
                            if (track_PODStatus == "Delivered")
                            {
                                track_PODStatus = "DELIVERED";
                            }
                        }
                        catch (Exception ex)
                        {
                            track_PODStatus = "";
                        }

                        


                        string strStatus_code = "";
                        string pstrInsertQuery = "";
                        try
                        {

                            if (track_PODStatus == "DELIVERED")
                            {
                                strStatus_code = "DEL";
                                deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);

                                System.Threading.Thread.Sleep(100);
                                if (nodelist.Count > 0)
                                {
                                    pstrInsertQuery = "insert into[XCARRIER_SHIPPING_VISIBILITY] (shipping_num, POD_DATETIME, POD_SIGN, POD_STATUS, Tracking_num, StatusCode, CURRENT_PLACE, CARRIER) values ";
                                }



                                for (int i = 0; i < nodelist.Count; i++)
                                {
                                    string ptrack_podLocation = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrStatus_code = "", ptrack_podsignature = "";

                                    DateTime? pdtPODDateTime = null;

                                    try
                                    {
                                        pstatusdatetime = nodelist[i].SelectSingleNode("ActivityDateTime").InnerText;
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    try
                                    {
                                        pdtPODDateTime = Convert.ToDateTime(pstatusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    try
                                    {
                                        ptrack_PODStatus = nodelist[i].SelectSingleNode("Activity").InnerText;
                                        if (ptrack_PODStatus == "Delivered")
                                        {
                                            ptrack_PODStatus = "Delivered";
                                            pstrStatus_code = "DEL";
                                        }
                                        else
                                        {
                                            // ptrack_PODStatus = "In-Transit";
                                            pstrStatus_code = "INT";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ptrack_PODStatus = "";
                                    }

                                    string pstrDatetime = null;
                                    try
                                    {
                                        pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                    }
                                    catch (Exception ex)
                                    {


                                    }
                                    try
                                    {
                                        ptrack_podsignature = xmlDoc.SelectSingleNode("Envelope/Body/GetByProNumberResponse/GetByProNumberResult/Signature").InnerText;

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    try
                                    {
                                        ptrack_podLocation = nodelist[i].SelectSingleNode("City").InnerText + ", " + nodelist[i].SelectSingleNode("State").InnerText;

                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    pstrInsertQuery = pstrInsertQuery + "('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'),";

                                }
                                if (pstrInsertQuery != "")
                                {
                                    try
                                    {
                                        pstrInsertQuery = pstrInsertQuery.Substring(0, pstrInsertQuery.Length - 1);
                                        addquery("SAIA pstrInsertQuery :: " + pstrInsertQuery + ":: TrackingNumber" + TrackingNo);
                                        SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                        if (con.State == ConnectionState.Closed)
                                        {
                                            con.Open();
                                        }
                                        SqlCmd.ExecuteNonQuery();

                                        if (con.State == ConnectionState.Open)
                                        {
                                            con.Close();
                                        }

                                        System.Threading.Thread.Sleep(100);
                                    }
                                    catch (Exception ex)
                                    {
                                        addquery("SAIA Ship Visibility Insert Exception :: TrackingNumber" + TrackingNo + " ::" + ex.ToString());

                                        if (con.State == ConnectionState.Open)
                                            con.Close();

                                    }
                                }
                            }
                            else
                            {
                                strStatus_code = "INT";
                            }

                        }
                        catch (Exception ex)
                        {
                            addquery("");
                            // strStatus_code = "";
                        }
                        XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);


                        

                    }
                }



            }
            catch (Exception ex)
            {
                addquery("Unable run FEDEX_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
            }

        }

        public static void FEDEX_FREIGHT_Status(object pparam_obj)
        {
            string fedexaccountno = "";
            string fedexmeterno = "";
            string fedexcspkey = "";
            string fedexcspwd = "";
            string fedexuserkey = "";
            string fedexuserpwd = "";
            string shippingno = "", ERPUpdateFlag = "", TrackingNo = "", ERPName = "", FeederSystemName = "", strShipFromCountry = "", Service = "", MastertrackingNo = "", DeliveryNUM = "", CustomerID = "", strDate = "", AccountNo = "", carrier = "", PlantID = "", connectionstring = "", erpenginepath = "", LabelPath = "";
            SqlConnection con = null;
            try
            {
                Array FEDEX_FREIGHTargArray = new object[2];
                FEDEX_FREIGHTargArray = (Array)pparam_obj;

                AccountNo = (string)FEDEX_FREIGHTargArray.GetValue(0);
                TrackingNo = (string)FEDEX_FREIGHTargArray.GetValue(1);
                PlantID = (string)FEDEX_FREIGHTargArray.GetValue(2);
                Service = (string)FEDEX_FREIGHTargArray.GetValue(3);
                connectionstring = (string)FEDEX_FREIGHTargArray.GetValue(4);
                strShipFromCountry = (string)FEDEX_FREIGHTargArray.GetValue(5);
                strDate = (string)FEDEX_FREIGHTargArray.GetValue(6);
                shippingno = (string)FEDEX_FREIGHTargArray.GetValue(7);
                erpenginepath = (string)FEDEX_FREIGHTargArray.GetValue(8);
                ERPUpdateFlag = (string)FEDEX_FREIGHTargArray.GetValue(9);
                LabelPath = (string)FEDEX_FREIGHTargArray.GetValue(10);
                DataSet dsCarrier = (DataSet)FEDEX_FREIGHTargArray.GetValue(11);
                con = (SqlConnection)FEDEX_FREIGHTargArray.GetValue(12);
                CustomerID = (string)FEDEX_FREIGHTargArray.GetValue(13);
                carrier = (string)FEDEX_FREIGHTargArray.GetValue(14);
                ERPName = (string)FEDEX_FREIGHTargArray.GetValue(15);
                DeliveryNUM = (string)FEDEX_FREIGHTargArray.GetValue(16);
                FeederSystemName = (string)FEDEX_FREIGHTargArray.GetValue(17);
                MastertrackingNo = (string)FEDEX_FREIGHTargArray.GetValue(18);

                DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number

                string strInsertQuery = "select ACCOUNTNO,METERNO,CSPUSERID,CSPPASSWORD,USERID,PASSWORD from XCARRIER_ACCOUNTBILLINGADDRESS where FEDEXFREIGHTACCOUNTNUMBER = '" + AccountNo + "'";


                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlDataAdapter da = new SqlDataAdapter(strInsertQuery, con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {

                    fedexaccountno = ds.Tables[0].Rows[0]["ACCOUNTNO"].ToString();
                    fedexmeterno = ds.Tables[0].Rows[0]["METERNO"].ToString();
                    fedexcspkey = ds.Tables[0].Rows[0]["CSPUSERID"].ToString();
                    fedexcspwd = ds.Tables[0].Rows[0]["CSPPASSWORD"].ToString();
                    fedexuserkey = ds.Tables[0].Rows[0]["USERID"].ToString();
                    fedexuserpwd = ds.Tables[0].Rows[0]["PASSWORD"].ToString();


                }

                if (con.State == ConnectionState.Open)
                    con.Close();


            }
            catch (Exception ex)
            {
                addquery("SQLUpdate Query failed for " + TrackingNo + " Reason : " + ex.Message.ToString());
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }




            try
            {


                string strXML = "";
                strXML = strXML + "<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns='http://fedex.com/ws/track/v14'>";
                strXML = strXML + "<soapenv:Header/>";
                strXML = strXML + "<soapenv:Body>";
                strXML = strXML + "<TrackRequest>";
                strXML = strXML + "<WebAuthenticationDetail>";
                strXML = strXML + "<ParentCredential>";
                strXML = strXML + "<Key>" + fedexcspkey + "</Key>";
                strXML = strXML + "<Password>" + fedexcspwd + "</Password>";
                strXML = strXML + "</ParentCredential>";
                strXML = strXML + "<UserCredential>";
                strXML = strXML + "<Key>" + fedexuserkey + "</Key>";
                strXML = strXML + "<Password>" + fedexuserpwd + "</Password>";
                strXML = strXML + "</UserCredential>";
                strXML = strXML + "</WebAuthenticationDetail>";
                strXML = strXML + "<ClientDetail>";
                strXML = strXML + "<AccountNumber>" + fedexaccountno + "</AccountNumber>";
                strXML = strXML + "<MeterNumber>" + fedexmeterno + "</MeterNumber>";
                strXML = strXML + "</ClientDetail>";
                strXML = strXML + "<TransactionDetail>";
                strXML = strXML + "<CustomerTransactionId>***Track v14 Request using VC#***</CustomerTransactionId>";
                strXML = strXML + "</TransactionDetail>";
                strXML = strXML + "<Version>";
                strXML = strXML + "<ServiceId>trck</ServiceId>";
                strXML = strXML + "<Major>14</Major>";
                strXML = strXML + "<Intermediate>0</Intermediate>";
                strXML = strXML + "<Minor>0</Minor>";
                strXML = strXML + "</Version>";
                strXML = strXML + "<SelectionDetails>";
                strXML = strXML + "<PackageIdentifier>";
                strXML = strXML + "<Type>TRACKING_NUMBER_OR_DOORTAG</Type>";
                strXML = strXML + "<Value>" + TrackingNo + "</Value>";
                strXML = strXML + "</PackageIdentifier>";
                strXML = strXML + "</SelectionDetails>";
                strXML = strXML + "<ProcessingOptions>INCLUDE_DETAILED_SCANS</ProcessingOptions>";
                strXML = strXML + "</TrackRequest>";
                strXML = strXML + "</soapenv:Body>";
                strXML = strXML + "</soapenv:Envelope>";

                try
                {
                    using (System.IO.StreamWriter outfile =
                    new System.IO.StreamWriter(LogPath + @"\FEDEX_FREIGHT_TrackRequest.xml"))
                    {
                        outfile.Write(strXML);
                        outfile.Close();
                        outfile.Dispose();
                        //GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                    addquery(ex.ToString());
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://ws.fedex.com/web-services/track");
                request.AllowAutoRedirect = false;

                request.Method = "POST";
                byte[] bytes = Encoding.UTF8.GetBytes(strXML);
                request.ContentLength = bytes.Length;
                request.ContentType = "text/xml;charset=UTF-8";

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                    requestStream.Dispose();

                }

                XmlDocument xmlDoc = new XmlDocument();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse();
                if (myResponse.StatusCode == HttpStatusCode.OK)
                {
                    Stream ReceiveStream = myResponse.GetResponseStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader readStream = new StreamReader(ReceiveStream, encode);
                    string Result = readStream.ReadToEnd();
                    readStream.Close();
                    readStream.Dispose();

                    Result = Result.Replace("SOAP-ENV:", "");
                    Result = Result.Replace(":SOAP-ENV", "");
                    Result = Result.Replace("<Envelope xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\">", "<Envelope>");
                    Result = Result.Replace("<TrackReply xmlns=\"http://fedex.com/ws/track/v14\">", "<TrackReply>");
                    xmlDoc.LoadXml(Result.Trim());
                    try
                    {
                        using (System.IO.StreamWriter outfile =
                        new System.IO.StreamWriter(LogPath + @"\FEDEX_FREIGHT_TrackResponse.xml"))
                        {
                            outfile.Write(Result);
                            outfile.Close();
                            outfile.Dispose();
                            //GC.Collect();
                        }
                    }
                    catch (Exception ex)
                    {
                        addquery(ex.ToString());
                    }


                    XmlNodeList nodelist = xmlDoc.SelectNodes("Envelope/Body/TrackReply/CompletedTrackDetails/TrackDetails/Events");
                    if (nodelist.Count > 0)
                    {

                        string track_podsignature = "";
                        string track_podLocation = "";
                        DateTime? dtPODDateTime = null;
                        string track_PODStatus = "";
                        int k = 0;
                        string statusdate = "", statusdatetime = "";
                        try
                        {
                            statusdatetime = nodelist[k].SelectSingleNode("Timestamp").InnerText;
                        }
                        catch (Exception ex)
                        {

                        }

                        try
                        {
                            dtPODDateTime = Convert.ToDateTime(statusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                        }
                        catch (Exception ex)
                        {

                        }
                        try
                        {
                            track_podsignature = xmlDoc.SelectSingleNode("Envelope/Body/TrackReply/CompletedTrackDetails/TrackDetails/DeliverySignatureName").InnerText;
                        }
                        catch (Exception ex)
                        {

                        }

                        try
                        {
                            track_podLocation = nodelist[k].SelectSingleNode("City").InnerText + ", " + nodelist[k].SelectSingleNode("State").InnerText;

                        }
                        catch (Exception ex)
                        {

                        }

                        try
                        {
                            track_PODStatus = nodelist[k].SelectSingleNode("Activity").InnerText;
                            if (track_PODStatus == "Delivered")
                            {
                                track_PODStatus = "DELIVERED";
                            }
                        }
                        catch (Exception ex)
                        {
                            track_PODStatus = "";
                        }
                        string strStatus_code = "";
                        string pstrInsertQuery = "";
                        try
                        {

                            if (track_PODStatus == "DELIVERED")
                            {
                                strStatus_code = "DEL";
                                deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);

                                System.Threading.Thread.Sleep(100);

                                if (nodelist.Count > 0)
                                {
                                    pstrInsertQuery = "insert into [XCARRIER_SHIPPING_VISIBILITY] (shipping_num,POD_DATETIME,POD_SIGN,POD_STATUS,Tracking_num,StatusCode,CURRENT_PLACE,CARRIER) values ";
                                }


                                for (int i = 0; i < nodelist.Count; i++)
                                {
                                    string ptrack_podLocation = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrStatus_code = "", ptrack_podsignature = "";
                                    DateTime? pdtPODDateTime = null;

                                    try
                                    {
                                        pstatusdatetime = nodelist[k].SelectSingleNode("Timestamp").InnerText;
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    try
                                    {
                                        pdtPODDateTime = Convert.ToDateTime(pstatusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    try
                                    {
                                        ptrack_PODStatus = nodelist[i].SelectSingleNode("EventDescription").InnerText;
                                        if (ptrack_PODStatus == "Delivered")
                                        {
                                            ptrack_PODStatus = "Delivered";
                                            pstrStatus_code = "DEL";
                                        }
                                        else
                                        {
                                            // ptrack_PODStatus = "In-Transit";
                                            pstrStatus_code = "INT";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ptrack_PODStatus = "";
                                    }

                                    string pstrDatetime = null;
                                    try
                                    {
                                        pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                    }
                                    catch (Exception ex)
                                    {


                                    }
                                    try
                                    {
                                        ptrack_podsignature = xmlDoc.SelectSingleNode("Envelope/Body/TrackReply/CompletedTrackDetails/TrackDetails/DeliverySignatureName").InnerText;

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    try
                                    {
                                        ptrack_podLocation = nodelist[k].SelectSingleNode("Address/City").InnerText + ", " + nodelist[k].SelectSingleNode("Address/StateOrProvinceCode").InnerText;

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    pstrInsertQuery = pstrInsertQuery + "('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'),";


                                }
                                if (pstrInsertQuery != "")
                                {
                                    try
                                    {
                                        pstrInsertQuery = pstrInsertQuery.Substring(0, pstrInsertQuery.Length - 1);
                                        addquery("FEDEX_FREIGHT pstrInsertQuery :: " + pstrInsertQuery + ":: TrackingNumber" + TrackingNo);

                                        SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                        if (con.State == ConnectionState.Closed)
                                        {
                                            con.Open();
                                        }
                                        SqlCmd.ExecuteNonQuery();
                                        if (con.State == ConnectionState.Open)
                                        {
                                            con.Close();
                                        }

                                        System.Threading.Thread.Sleep(100);

                                    }
                                    catch (Exception ex)
                                    {
                                        addquery("FEDEX_FREIGHT Ship Visibility Insert Exception :: TrackingNumber" + TrackingNo + " ::" + ex.ToString());

                                        if (con.State == ConnectionState.Open)
                                            con.Close();
                                    }
                                }
                            }
                            else
                            {
                                strStatus_code = "INT";
                            }

                        }
                        catch (Exception ex)
                        {
                            // strStatus_code = "";
                        }


                        XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);

                        

                    }
                }



            }
            catch (Exception ex)
            {
                addquery("Unable run FEDEX_FREIGHT_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
            }

        }

        private static string GetAccessToken(string tokenEndpoint, string clientId, string clientSecret)
        {
            // Convert client ID and client secret to Base64 for Basic Authentication
            string base64Auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            // Create HttpWebRequest
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
            request.Method = "POST";
            request.Headers["Authorization"] = "Basic " + base64Auth;
            request.ContentType = "application/x-www-form-urlencoded";

            // Set the request body
            string requestBody = "grant_type=client_credentials";
            byte[] data = Encoding.UTF8.GetBytes(requestBody);
            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Assuming the response is in JSON format
                        var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.ReadToEnd());
                        return responseData["sessionToken"];
                    }
                    else
                    {
                        throw new Exception($"Failed to get access token. Status Code: {response.StatusCode}");
                    }
                }
            }
            catch (WebException ex)
            {
                // Handle exceptions here
                throw new Exception($"Error in HTTP request: {ex.Message}");
            }
        }

        private static string GetAccessToken_fedex(string clientId, string clientSecret, string tokenEndpoint)
        {
            string token_generated = "";
            var parameters = new NameValueCollection
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "grant_type", "client_credentials" }
        };

            var requestBody = ToQueryString(parameters);
            var request = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(requestBody);
                streamWriter.Flush();
                streamWriter.Close();
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Assuming the response is in JSON format
                        var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.ReadToEnd());
                        token_generated = responseData["access_token"];
                    }
                    else
                    {
                        addquery($"Failed to get access token. Status Code: {response.StatusCode}");
                    }
                }
            }
            catch (WebException ex)
            {
                // Handle exceptions here
                addquery($"Error in HTTP request: {ex.Message}");
                return null;
            }
            return token_generated;
        }
        private static string ToQueryString(NameValueCollection parameters)
        {
            var stringBuilder = new StringBuilder();
            foreach (string key in parameters.AllKeys)
            {
                stringBuilder.Append(Uri.EscapeDataString(key));
                stringBuilder.Append("=");
                stringBuilder.Append(Uri.EscapeDataString(parameters[key]));
                stringBuilder.Append("&");
            }
            return stringBuilder.ToString().TrimEnd('&');
        }

        public static void ODFL_Status(object pparam_obj)
        {
            try
            {
                string ODFLaccountno = "";
                string ODFLuserkey = "";
                string ODFLuserpwd = "";
                string shippingno = "", ERPUpdateFlag = "", TrackingNo = "", ERPName = "", FeederSystemName = "", strShipFromCountry = "", Service = "", MastertrackingNo = "", DeliveryNUM = "", CustomerID = "", strDate = "", AccountNo = "", carrier = "", PlantID = "", connectionstring = "", erpenginepath = "", LabelPath = "";
                SqlConnection con = null;

                Array ODFLargArray = new object[2];
                ODFLargArray = (Array)pparam_obj;
                AccountNo = (string)ODFLargArray.GetValue(0);
                TrackingNo = (string)ODFLargArray.GetValue(1);
                PlantID = (string)ODFLargArray.GetValue(2);
                Service = (string)ODFLargArray.GetValue(3);
                connectionstring = (string)ODFLargArray.GetValue(4);
                strShipFromCountry = (string)ODFLargArray.GetValue(5);
                strDate = (string)ODFLargArray.GetValue(6);
                shippingno = (string)ODFLargArray.GetValue(7);
                erpenginepath = (string)ODFLargArray.GetValue(8);
                ERPUpdateFlag = (string)ODFLargArray.GetValue(9);
                LabelPath = (string)ODFLargArray.GetValue(10);
                DataSet dsCarrier = (DataSet)ODFLargArray.GetValue(11);
                con = (SqlConnection)ODFLargArray.GetValue(12);
                CustomerID = (string)ODFLargArray.GetValue(13);
                carrier = (string)ODFLargArray.GetValue(14);
                ERPName = (string)ODFLargArray.GetValue(15);
                DeliveryNUM = (string)ODFLargArray.GetValue(16);
                FeederSystemName = (string)ODFLargArray.GetValue(17);
                MastertrackingNo = (string)ODFLargArray.GetValue(18);

                DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number


                if (dataRows.Length > 0)
                {
                    ODFLuserkey = dataRows[0]["USER_ID"].ToString();
                    ODFLuserpwd = dataRows[0]["PASSWORD"].ToString();
                    ODFLaccountno = dataRows[0]["ACCOUNT_NUMBER"].ToString();

                }



                string tokenEndpoint = "https://api.odfl.com/auth/v1.0/token";
                string trackingEndpoint = "https://api.odfl.com/tracking/v2.0/shipment.track";

                string token = "";
                string referenceType = "PRO";

                try
                {
                    token = GetAccessToken(tokenEndpoint, ODFLuserkey, ODFLuserpwd);
                    // Call the tracking endpoint with the obtained token
                    referenceType = "PRO";


                }
                catch (Exception ex)
                {
                    addquery($"Error: {ex.Message}");
                }


                try
                {
                    // Create HttpWebRequest for tracking
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(trackingEndpoint);
                    request.Method = "POST";
                    request.Headers["Authorization"] = "Bearer " + token;
                    request.ContentType = "application/json";

                    // Set the request body for shipment tracking
                    string requestBody = $"{{\"referenceType\": \"{referenceType}\", \"referenceNumber\": \"{TrackingNo}\"}}";
                    byte[] data = Encoding.UTF8.GetBytes(requestBody);
                    request.ContentLength = data.Length;

                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    // Get the response
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            // Process the tracking response as needed

                            string jsonResponse = reader.ReadToEnd();
                            string XMLResponse = ConvertJsonToXml(jsonResponse);
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(XMLResponse.Trim());

                            try
                            {
                                using (System.IO.StreamWriter outfile =
                                new System.IO.StreamWriter(LogPath + @"\" + TrackingNo + "_ODFLTRACK.xml"))
                                {
                                    outfile.Write(XMLResponse);
                                    outfile.Close();
                                    outfile.Dispose();
                                    //GC.Collect();
                                }
                            }
                            catch (Exception ex)
                            {
                                addquery(ex.ToString());
                            }


                            XmlNodeList nodelist = xmlDoc.SelectNodes("Root/traceInfo/trackTraceDetail");
                            if (nodelist.Count > 0)
                            {

                                string track_podsignature = "";
                                string track_podLocation = "";
                                DateTime? dtPODDateTime = null;
                                string track_PODStatus = "";
                                int k = 0;
                                string statusdate = "", statusdatetime = "";
                                try
                                {
                                    statusdatetime = nodelist[k].SelectSingleNode("dateTime").InnerText;
                                }
                                catch (Exception ex)
                                {

                                }

                                try
                                {
                                    dtPODDateTime = Convert.ToDateTime(statusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                }
                                catch (Exception ex)
                                {

                                }
                                try
                                {
                                    track_podsignature = "";
                                }
                                catch (Exception ex)
                                {

                                }

                                if (nodelist[k].SelectSingleNode("city") != null)
                                {
                                    try
                                    {
                                        track_podLocation = nodelist[k].SelectSingleNode("city").InnerText + ", " + nodelist[k].SelectSingleNode("state").InnerText;

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }

                                try
                                {
                                    track_PODStatus = nodelist[k].SelectSingleNode("status").InnerText;
                                    if (track_PODStatus == "Delivered")
                                    {
                                        track_PODStatus = "DELIVERED";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    track_PODStatus = "";
                                }
                                string strStatus_code = "";
                                string pstrInsertQuery = "";
                                try
                                {

                                    if (track_PODStatus == "DELIVERED")
                                    {
                                        strStatus_code = "DEL";
                                        deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);

                                        System.Threading.Thread.Sleep(100);

                                        if (nodelist.Count > 0)
                                        {
                                            pstrInsertQuery = "insert into [XCARRIER_SHIPPING_VISIBILITY] (shipping_num,POD_DATETIME,POD_SIGN,POD_STATUS,Tracking_num,StatusCode,CURRENT_PLACE,CARRIER) values ";
                                        }


                                        for (int i = 0; i < nodelist.Count; i++)
                                        {
                                            string ptrack_podLocation = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrStatus_code = "", ptrack_podsignature = "";
                                            DateTime? pdtPODDateTime = null;

                                            try
                                            {
                                                pstatusdatetime = nodelist[k].SelectSingleNode("dateTime").InnerText;
                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            try
                                            {
                                                pdtPODDateTime = Convert.ToDateTime(pstatusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            try
                                            {
                                                ptrack_PODStatus = nodelist[i].SelectSingleNode("status").InnerText;
                                                if (ptrack_PODStatus == "Delivered")
                                                {
                                                    ptrack_PODStatus = "Delivered";
                                                    pstrStatus_code = "DEL";
                                                }
                                                else
                                                {
                                                    // ptrack_PODStatus = "In-Transit";
                                                    pstrStatus_code = "INT";
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                ptrack_PODStatus = "";
                                            }

                                            string pstrDatetime = null;
                                            try
                                            {
                                                pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                            }
                                            catch (Exception ex)
                                            {


                                            }
                                            try
                                            {
                                                ptrack_podsignature = ""; ;

                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                ptrack_podLocation = nodelist[k].SelectSingleNode("city").InnerText + ", " + nodelist[k].SelectSingleNode("state").InnerText;

                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            pstrInsertQuery = pstrInsertQuery + "('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'),";


                                        }
                                        if (pstrInsertQuery != "")
                                        {
                                            try
                                            {
                                                pstrInsertQuery = pstrInsertQuery.Substring(0, pstrInsertQuery.Length - 1);
                                                addquery("FEDEX_FREIGHT pstrInsertQuery :: " + pstrInsertQuery + ":: TrackingNumber" + TrackingNo);

                                                SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                                if (con.State == ConnectionState.Closed)
                                                {
                                                    con.Open();
                                                }
                                                SqlCmd.ExecuteNonQuery();
                                                if (con.State == ConnectionState.Open)
                                                {
                                                    con.Close();
                                                }

                                                System.Threading.Thread.Sleep(100);

                                            }
                                            catch (Exception ex)
                                            {
                                                addquery("ODFL Ship Visibility Insert Exception :: TrackingNumber" + TrackingNo + " ::" + ex.ToString());

                                                if (con.State == ConnectionState.Open)
                                                    con.Close();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        strStatus_code = "INT";
                                    }

                                }
                                catch (Exception ex)
                                {
                                    // strStatus_code = "";
                                }


                                XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);




                            }

                        }
                        else
                        {
                            addquery($"Failed to track shipment. Status Code: {response.StatusCode}");
                        }
                    }
                }
                catch (Exception ex)
                {


                }
            }
            catch (Exception ex)
            {

                addquery(ex.Message);
            }




        }

        public static void FexExRestAPI_Status(object pparam_obj)
        {

            string client_id = "";
            string client_secret = "";
            string grantType = "client_credentials";
            string shippingno = "", ERPUpdateFlag = "", TrackingNo = "", ERPName = "", FeederSystemName = "", strShipFromCountry = "", Service = "", MastertrackingNo = "", DeliveryNUM = "", CustomerID = "", strDate = "", AccountNo = "", carrier = "", PlantID = "", connectionstring = "", erpenginepath = "", LabelPath = "";
            SqlConnection con = null;
            try
            {
                Array ODFLargArray = new object[2];
                ODFLargArray = (Array)pparam_obj;

                AccountNo = (string)ODFLargArray.GetValue(0);
                TrackingNo = (string)ODFLargArray.GetValue(1);
                PlantID = (string)ODFLargArray.GetValue(2);
                Service = (string)ODFLargArray.GetValue(3);
                connectionstring = (string)ODFLargArray.GetValue(4);
                strShipFromCountry = (string)ODFLargArray.GetValue(5);
                strDate = (string)ODFLargArray.GetValue(6);
                shippingno = (string)ODFLargArray.GetValue(7);
                erpenginepath = (string)ODFLargArray.GetValue(8);
                ERPUpdateFlag = (string)ODFLargArray.GetValue(9);
                LabelPath = (string)ODFLargArray.GetValue(10);
                DataSet dsCarrier = (DataSet)ODFLargArray.GetValue(11);
                con = (SqlConnection)ODFLargArray.GetValue(12);
                CustomerID = (string)ODFLargArray.GetValue(13);
                carrier = (string)ODFLargArray.GetValue(14);
                ERPName = (string)ODFLargArray.GetValue(15);
                DeliveryNUM = (string)ODFLargArray.GetValue(16);
                FeederSystemName = (string)ODFLargArray.GetValue(17);
                MastertrackingNo = (string)ODFLargArray.GetValue(18);

                DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number

                string strInsertQuery = "select ACCOUNTNO,METERNO,CSPUSERID,CSPPASSWORD,USERID,PASSWORD from XCARRIER_ACCOUNTBILLINGADDRESS where FEDEXFREIGHTACCOUNTNUMBER = '" + AccountNo + "'";


                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlDataAdapter da = new SqlDataAdapter(strInsertQuery, con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    client_id = ds.Tables[0].Rows[0]["USERID"].ToString();
                    client_secret = ds.Tables[0].Rows[0]["ACCOUNTNO"].ToString();

                }

                if (con.State == ConnectionState.Open)
                    con.Close();


            }
            catch (Exception ex)
            {
                addquery("SQLUpdate Query failed for " + TrackingNo + " Reason : " + ex.Message.ToString());
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            string tokenEndpoint = "https://apis-sandbox.fedex.com/oauth/token";
            string apiEndpoint = "https://apis-sandbox.fedex.com/track/v1/trackingnumbers";

            string token = "";

            try
            {
                token = GetAccessToken(tokenEndpoint, client_id, client_secret);
                // Call the tracking endpoint with the obtained token

            }
            catch (Exception ex)
            {
                addquery($"Error: {ex.Message}");
            }

            try
            {
                var requestPayload = new
                {
                    includeDetailedScans = true,
                    trackingInfo = new[]
                   {
                       new
                      {
                           trackingNumberInfo = new
                           {
                            trackingNumber = "794686876571"
                           }
                      }
                   }
                };
                var jsonPayload = JsonConvert.SerializeObject(requestPayload);
                var request = (HttpWebRequest)WebRequest.Create(apiEndpoint);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", $"Bearer {token}");

                // Write the JSON payload to the request stream
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jsonPayload);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                // Get the response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Process the tracking response as needed

                        string jsonResponse = reader.ReadToEnd();
                        string XMLResponse = ConvertJsonToXml(jsonResponse);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(XMLResponse.Trim());

                        try
                        {
                            using (System.IO.StreamWriter outfile =
                             new System.IO.StreamWriter(LogPath + @"\" + TrackingNo + "FedEx_RESTAPI_Track_Response.xml"))
                            {
                                outfile.Write(XMLResponse);
                                outfile.Close();
                                outfile.Dispose();
                                //GC.Collect();
                            }
                        }
                        catch (Exception ex)
                        {
                            addquery(ex.ToString());
                        }


                        XmlNodeList nodelist = xmlDoc.SelectNodes("Root/output/completeTrackResults/trackResults/scanEvents");
                        if (nodelist.Count > 0)
                        {

                            string track_podsignature = "";
                            string track_podLocation = "";
                            DateTime? dtPODDateTime = null;
                            string track_PODStatus = "";
                            int k = 0;
                            string statusdate = "", statusdatetime = "";
                            try
                            {
                                statusdatetime = nodelist[k].SelectSingleNode("date").InnerText;
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                dtPODDateTime = Convert.ToDateTime(statusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                            }
                            catch (Exception ex)
                            {

                            }
                            try
                            {
                                track_podsignature = "";
                            }
                            catch (Exception ex)
                            {

                            }

                            if (nodelist[k].SelectSingleNode("scanLocation/city") != null)
                            {
                                try
                                {
                                    track_podLocation = nodelist[k].SelectSingleNode("scanLocation/city").InnerText + ", " + nodelist[k].SelectSingleNode("scanLocation/stateOrProvinceCode").InnerText;

                                }
                                catch (Exception ex)
                                {

                                }
                            }

                            try
                            {
                                track_PODStatus = nodelist[k].SelectSingleNode("eventDescription").InnerText;
                                if (track_PODStatus == "Delivered")
                                {
                                    track_PODStatus = "DELIVERED";
                                }
                            }
                            catch (Exception ex)
                            {
                                track_PODStatus = "";
                            }
                            string strStatus_code = "";
                            string pstrInsertQuery = "";
                            try
                            {

                                if (track_PODStatus == "DELIVERED")
                                {
                                    strStatus_code = "DEL";
                                    deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);

                                    System.Threading.Thread.Sleep(100);

                                    if (nodelist.Count > 0)
                                    {
                                        pstrInsertQuery = "insert into [XCARRIER_SHIPPING_VISIBILITY] (shipping_num,POD_DATETIME,POD_SIGN,POD_STATUS,Tracking_num,StatusCode,CURRENT_PLACE,CARRIER) values ";
                                    }


                                    for (int i = 0; i < nodelist.Count; i++)
                                    {
                                        string ptrack_podLocation = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrStatus_code = "", ptrack_podsignature = "";
                                        DateTime? pdtPODDateTime = null;

                                        try
                                        {
                                            pstatusdatetime = nodelist[k].SelectSingleNode("dateTime").InnerText;
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            pdtPODDateTime = Convert.ToDateTime(pstatusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            ptrack_PODStatus = nodelist[i].SelectSingleNode("eventDescription").InnerText;
                                            if (ptrack_PODStatus == "Delivered")
                                            {
                                                ptrack_PODStatus = "Delivered";
                                                pstrStatus_code = "DEL";
                                            }
                                            else
                                            {
                                                // ptrack_PODStatus = "In-Transit";
                                                pstrStatus_code = "INT";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ptrack_PODStatus = "";
                                        }

                                        string pstrDatetime = null;
                                        try
                                        {
                                            pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                        }
                                        catch (Exception ex)
                                        {


                                        }
                                        try
                                        {
                                            ptrack_podsignature = ""; ;

                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            ptrack_podLocation = nodelist[k].SelectSingleNode("scanLocation/city").InnerText + ", " + nodelist[k].SelectSingleNode("scanLocation/stateOrProvinceCode").InnerText;

                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        pstrInsertQuery = pstrInsertQuery + "('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'),";


                                    }
                                    if (pstrInsertQuery != "")
                                    {
                                        try
                                        {
                                            pstrInsertQuery = pstrInsertQuery.Substring(0, pstrInsertQuery.Length - 1);
                                            addquery("FEDEX_FREIGHT pstrInsertQuery :: " + pstrInsertQuery + ":: TrackingNumber" + TrackingNo);

                                            SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                            if (con.State == ConnectionState.Closed)
                                            {
                                                con.Open();
                                            }
                                            SqlCmd.ExecuteNonQuery();
                                            if (con.State == ConnectionState.Open)
                                            {
                                                con.Close();
                                            }

                                            System.Threading.Thread.Sleep(100);

                                        }
                                        catch (Exception ex)
                                        {
                                            addquery("FedEx REST API Ship Visibility Insert Exception :: TrackingNumber" + TrackingNo + " ::" + ex.ToString());

                                            if (con.State == ConnectionState.Open)
                                                con.Close();
                                        }
                                    }
                                }
                                else
                                {
                                    strStatus_code = "INT";
                                }

                            }
                            catch (Exception ex)
                            {
                                // strStatus_code = "";
                            }

                            XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);

                        }

                    }
                    else
                    {
                        addquery($"Failed to track shipment. Status Code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {


            }




        }

        static string ConvertJsonToXml(string jsonResponse)
        {
            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(jsonResponse);
            return JsonConvert.DeserializeXNode(jsonObject.ToString(), "Root").ToString();
        }
        public static void DAYTONFREIGHT_Status(object pparam_obj)
        {
            string UserId = "";
            string Password = "";
            string AuthHeader = "";
            DataSet DAYTONFREIGHTDS = new DataSet();
            string shippingno = "", ERPUpdateFlag = "", TrackingNo = "", ERPName = "", FeederSystemName = "", strShipFromCountry = "", Service = "", MastertrackingNo = "", DeliveryNUM = "", CustomerID = "", strDate = "", AccountNo = "", carrier = "", PlantID = "", connectionstring = "", erpenginepath = "", LabelPath = "";
            SqlConnection con = null;
            DAYTONFREIGHTDS.Clear();
            try
            {
                Array DAYTONFREIGHTargArray = new object[2];
                DAYTONFREIGHTargArray = (Array)pparam_obj;

                AccountNo = (string)DAYTONFREIGHTargArray.GetValue(0);
                TrackingNo = (string)DAYTONFREIGHTargArray.GetValue(1);
                PlantID = (string)DAYTONFREIGHTargArray.GetValue(2);
                Service = (string)DAYTONFREIGHTargArray.GetValue(3);
                connectionstring = (string)DAYTONFREIGHTargArray.GetValue(4);
                strShipFromCountry = (string)DAYTONFREIGHTargArray.GetValue(5);
                strDate = (string)DAYTONFREIGHTargArray.GetValue(6);
                shippingno = (string)DAYTONFREIGHTargArray.GetValue(7);
                erpenginepath = (string)DAYTONFREIGHTargArray.GetValue(8);
                ERPUpdateFlag = (string)DAYTONFREIGHTargArray.GetValue(9);
                LabelPath = (string)DAYTONFREIGHTargArray.GetValue(10);
                DataSet dsCarrier = (DataSet)DAYTONFREIGHTargArray.GetValue(11);
                con = (SqlConnection)DAYTONFREIGHTargArray.GetValue(12);
                CustomerID = (string)DAYTONFREIGHTargArray.GetValue(13);
                carrier = (string)DAYTONFREIGHTargArray.GetValue(14);
                ERPName = (string)DAYTONFREIGHTargArray.GetValue(15);
                DeliveryNUM = (string)DAYTONFREIGHTargArray.GetValue(16);
                FeederSystemName = (string)DAYTONFREIGHTargArray.GetValue(17);
                MastertrackingNo = (string)DAYTONFREIGHTargArray.GetValue(18);

                DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number

                if (dataRows.Length > 0)
                {

                    UserId = dataRows[0]["USER_ID"].ToString();
                    Password = dataRows[0]["PASSWORD"].ToString();

                }

                try
                {
                    string URL = "https://api.daytonfreight.com/public/3/api/tracking/bynumber?";/// //https://api.daytonfreight.com/public/3/api/tracking/bynumber?number=691710688&type=0
                    URL = URL + "number=" + TrackingNo + "&" + "type=0";
                    AuthHeader = UserId + ":" + Password;

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL);
                    request.PreAuthenticate = true;
                    request.AllowAutoRedirect = false;
                    request.Method = "GET";
                    byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes(AuthHeader);
                    string encodedAuthHeaderText = Convert.ToBase64String(bytesToEncode);
                    request.ContentType = "application/json";
                    request.Headers.Add("Authorization", "Basic " + encodedAuthHeaderText);



                    XmlDocument xmlDoc = new XmlDocument();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse();
                    if (myResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Stream ReceiveStream = myResponse.GetResponseStream();
                        Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                        StreamReader readStream = new StreamReader(ReceiveStream, encode);
                        string Result = readStream.ReadToEnd();
                        readStream.Close();
                        readStream.Dispose();

                        Result = "{\"root\":{" + Result.Trim().TrimStart('{').TrimEnd('}') + "}";
                        XmlDocument doc = JsonConvert.DeserializeXmlNode(Result);


                        try
                        {
                            using (System.IO.StreamWriter outfile =
                            new System.IO.StreamWriter(LogPath + @"\DAYTONFREIGHT_TrackResponse.xml"))
                            {
                                outfile.Write(Result);
                                outfile.Close();
                                outfile.Dispose();
                                //GC.Collect();
                            }
                        }
                        catch (Exception ex)
                        {
                            addquery(ex.ToString());
                        }



                        string track_podLocation = "";
                        DateTime? dtPODDateTime = null;
                        string track_PODStatus = "", track_podsignature = "";
                        string strStatus_code = "";
                        int k = 0;
                        string statusdatetime = "";
                        string statustime = "";
                        string statusdate = "";
                        DateTime time;

                        XmlNodeList statusNodelist = doc.SelectNodes("root/results/status");

                        if (statusNodelist.Count > 0)
                        {
                            try
                            {
                                statusdatetime = statusNodelist[k].SelectSingleNode("time").InnerText.Replace("T", " ");
                            }
                            catch (Exception ex)
                            {
                            }

                            try
                            {

                                dtPODDateTime = Convert.ToDateTime(statusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                track_podsignature = statusNodelist[k].SelectSingleNode("signedBy").InnerText;
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                track_podLocation = statusNodelist[k].SelectSingleNode("city").InnerText + ", " + statusNodelist[k].SelectSingleNode("state").InnerText;

                            }
                            catch (Exception ex)
                            {

                            }



                            try
                            {
                                string pstrInsertQuery = "";
                                track_PODStatus = statusNodelist[k].SelectSingleNode("activity").InnerText.Trim();
                                if (track_PODStatus.ToUpper() == "DELIVERED TO DESTINATION" || track_PODStatus.ToUpper() == "DELIVERED")
                                {
                                    track_PODStatus = "DELIVERED";
                                    strStatus_code = "DEL";
                                    deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);// ===== Deleting the existed record.
                                    System.Threading.Thread.Sleep(100);
                                    if (statusNodelist.Count > 0)
                                    {
                                        pstrInsertQuery = "insert into[XCARRIER_SHIPPING_VISIBILITY] (shipping_num, POD_DATETIME, POD_SIGN, POD_STATUS, Tracking_num, StatusCode, CURRENT_PLACE, CARRIER) values ";
                                    }

                                    for (int i = 0; i < statusNodelist.Count; i++)
                                    {
                                        string ptrack_podLocation = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrStatus_code = "", ptrack_podsignature = "", pstatusdate = "", pstatustime = "";
                                        DateTime? pdtPODDateTime = null;
                                        DateTime ptime;

                                        try
                                        {
                                            pstatusdatetime = statusNodelist[i].SelectSingleNode("time").InnerText.Replace("T", " ");
                                        }
                                        catch (Exception ex)
                                        {
                                        }



                                        try
                                        {
                                            pdtPODDateTime = Convert.ToDateTime(pstatusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            ptrack_PODStatus = statusNodelist[i].SelectSingleNode("activity").InnerText.Trim();
                                            if (ptrack_PODStatus.ToUpper() == "DELIVERED" || ptrack_PODStatus.ToUpper() == "DELIVERED TO DESTINATION")
                                            {
                                                ptrack_PODStatus = "Delivered";
                                                pstrStatus_code = "DEL";
                                            }
                                            else
                                            {
                                                // ptrack_PODStatus = "In-Transit";
                                                pstrStatus_code = "INT";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ptrack_PODStatus = "";
                                        }

                                        string pstrDatetime = null;
                                        try
                                        {
                                            pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            ptrack_podLocation = statusNodelist[i].SelectSingleNode("city").InnerText + ", " + statusNodelist[i].SelectSingleNode("state").InnerText;

                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        pstrInsertQuery = pstrInsertQuery + "('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'),";

                                    }
                                    if (pstrInsertQuery != "")
                                    {
                                        try
                                        {
                                            pstrInsertQuery = pstrInsertQuery.Substring(0, pstrInsertQuery.Length - 1);

                                            SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                            if (con.State == ConnectionState.Closed)
                                            {
                                                con.Open();
                                            }
                                            SqlCmd.ExecuteNonQuery();

                                            if (con.State == ConnectionState.Open)
                                            {
                                                con.Close();
                                            }

                                            System.Threading.Thread.Sleep(100);
                                        }
                                        catch (Exception ex)
                                        {

                                            if (con.State == ConnectionState.Open)
                                                con.Close();

                                        }
                                    }



                                }
                                else
                                {
                                    strStatus_code = "INT";
                                }
                            }
                            catch (Exception ex)
                            {
                                track_PODStatus = "";
                            }
                            XCARRIERUPDATE(con, dtPODDateTime, "", track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);

                        }

                    }

                }
                catch (Exception ex)
                {
                    addquery("Unable run RL_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
                }

            }
            catch (Exception ex)
            {
                addquery("Unable run RL_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
            }

        }

        public static void RL_Status(object pparam_obj)
        {
            string RLAPIkey = "";
            DataSet RLDS = new DataSet();
            string shippingno = "", ERPUpdateFlag = "", TrackingNo = "", ERPName = "", FeederSystemName = "", strShipFromCountry = "", Service = "", MastertrackingNo = "", DeliveryNUM = "", CustomerID = "", strDate = "", AccountNo = "", carrier = "", PlantID = "", connectionstring = "", erpenginepath = "", LabelPath = "";
            SqlConnection con = null;
            RLDS.Clear();
            try
            {
                Array RLargArray = new object[2];
                RLargArray = (Array)pparam_obj;

                AccountNo = (string)RLargArray.GetValue(0);
                TrackingNo = (string)RLargArray.GetValue(1);
                PlantID = (string)RLargArray.GetValue(2);
                Service = (string)RLargArray.GetValue(3);
                connectionstring = (string)RLargArray.GetValue(4);
                strShipFromCountry = (string)RLargArray.GetValue(5);
                strDate = (string)RLargArray.GetValue(6);
                shippingno = (string)RLargArray.GetValue(7);
                erpenginepath = (string)RLargArray.GetValue(8);
                ERPUpdateFlag = (string)RLargArray.GetValue(9);
                LabelPath = (string)RLargArray.GetValue(10);
                DataSet dsCarrier = (DataSet)RLargArray.GetValue(11);
                con = (SqlConnection)RLargArray.GetValue(12);
                CustomerID = (string)RLargArray.GetValue(13);
                carrier = (string)RLargArray.GetValue(14);
                ERPName = (string)RLargArray.GetValue(15);
                DeliveryNUM = (string)RLargArray.GetValue(16);
                FeederSystemName = (string)RLargArray.GetValue(17);
                MastertrackingNo = (string)RLargArray.GetValue(18);

                DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number

                if (dataRows.Length > 0)
                {
                    RLAPIkey = dataRows[0]["METER_NUMBER"].ToString();
                }

                try
                {

                    string sxml = "";
                    sxml = "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:rlc=\"http://www.rlcarriers.com/\">";
                    sxml += "<soap:Header/>" + System.Environment.NewLine; ;
                    sxml += "<soap:Body>" + System.Environment.NewLine; ;
                    sxml += "<rlc:TraceShipment>";
                    sxml += "<rlc:APIKey>" + RLAPIkey + "</rlc:APIKey>" + System.Environment.NewLine; ;
                    sxml += "<rlc:request>" + System.Environment.NewLine; ;
                    sxml += "<rlc:TraceNumbers>" + System.Environment.NewLine; ;
                    sxml += "<rlc:string>" + TrackingNo + "</rlc:string>" + System.Environment.NewLine; ;
                    sxml += "</rlc:TraceNumbers>" + System.Environment.NewLine; ;
                    sxml += "<rlc:TraceType>PRO</rlc:TraceType>" + System.Environment.NewLine; ;
                    sxml += "</rlc:request>" + System.Environment.NewLine; ;
                    sxml += "</rlc:TraceShipment>" + System.Environment.NewLine; ;
                    sxml += "</soap:Body>" + System.Environment.NewLine; ;
                    sxml += "</soap:Envelope>";

                    try
                    {
                        using (System.IO.StreamWriter outfile =
                        new System.IO.StreamWriter(LogPath + @"\RL_TrackRequest.xml"))
                        {
                            outfile.Write(sxml);
                            outfile.Close();
                            outfile.Dispose();
                            //GC.Collect();
                        }
                    }
                    catch (Exception ex)
                    {
                        addquery(ex.ToString());
                    }
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://api.rlcarriers.com/1.0.3/ShipmentTracingService.asmx");
                    request.PreAuthenticate = true;
                    request.AllowAutoRedirect = false;
                    request.Host = "api.rlcarriers.com";
                    request.Method = "POST";
                    byte[] bytes = Encoding.UTF8.GetBytes(sxml);
                    request.ContentLength = bytes.Length;
                    request.ContentType = "text/xml;charset=UTF-8";

                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Close();
                        requestStream.Dispose();

                    }

                    XmlDocument xmlDoc = new XmlDocument();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse();
                    if (myResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Stream ReceiveStream = myResponse.GetResponseStream();
                        Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                        StreamReader readStream = new StreamReader(ReceiveStream, encode);
                        string Result = readStream.ReadToEnd();
                        readStream.Close();
                        readStream.Dispose();

                        xmlDoc = new System.Xml.XmlDocument();
                        xmlDoc.LoadXml(Result);
                        RLDS.ReadXml(new XmlTextReader(new System.IO.StringReader(xmlDoc.InnerXml)));


                        try
                        {
                            using (System.IO.StreamWriter outfile =
                            new System.IO.StreamWriter(LogPath + @"\RL_TrackResponse.xml"))
                            {
                                outfile.Write(Result);
                                outfile.Close();
                                outfile.Dispose();
                                //GC.Collect();
                            }
                        }
                        catch (Exception ex)
                        {
                            addquery(ex.ToString());
                        }



                        string track_podLocation = "";
                        DateTime? dtPODDateTime = null;
                        string track_PODStatus = "";
                        string strStatus_code = "";
                        int k = 0;
                        string statusdatetime = "";
                        string statustime = "";
                        string statusdate = "";
                        DateTime time;
                        if (RLDS.Tables["Status"].Rows.Count > 0)
                        {

                            try
                            {
                                statusdate = RLDS.Tables["Status"].Rows[RLDS.Tables["Status"].Rows.Count - 1]["Date"].ToString().Trim();
                            }
                            catch (Exception ex)
                            {
                            }

                            try
                            {
                                statustime = RLDS.Tables["Status"].Rows[RLDS.Tables["Status"].Rows.Count - 1]["Time"].ToString().Trim().Replace("AM", "").Replace("PM", "");

                                statusdatetime = statusdate + " " + statustime.ToString();

                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {

                                dtPODDateTime = Convert.ToDateTime(statusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                track_podLocation = "";

                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                string pstrInsertQuery = "";
                                track_PODStatus = RLDS.Tables["Status"].Rows[RLDS.Tables["Status"].Rows.Count - 1]["Description"].ToString().Trim();
                                if (track_PODStatus.ToUpper() == "DELIVERED")
                                {
                                    track_PODStatus = "DELIVERED";
                                    strStatus_code = "DEL";
                                    deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);// ===== Deleting the existed record.
                                    System.Threading.Thread.Sleep(100);
                                    if (RLDS.Tables["Status"].Rows.Count > 0)
                                    {
                                        pstrInsertQuery = "insert into[XCARRIER_SHIPPING_VISIBILITY] (shipping_num, POD_DATETIME, POD_SIGN, POD_STATUS, Tracking_num, StatusCode, CURRENT_PLACE, CARRIER) values ";
                                    }

                                    for (int i = 0; i < RLDS.Tables["Status"].Rows.Count; i++)
                                    {
                                        string ptrack_podLocation = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrStatus_code = "", ptrack_podsignature = "", pstatusdate = "", pstatustime = "";
                                        DateTime? pdtPODDateTime = null;
                                        DateTime ptime;

                                        try
                                        {
                                            pstatusdate = RLDS.Tables["Status"].Rows[i]["Date"].ToString().Trim();
                                        }
                                        catch (Exception ex)
                                        {
                                        }

                                        try
                                        {
                                            pstatustime = RLDS.Tables["Status"].Rows[i]["Time"].ToString().Trim().Replace("AM", "").Replace("PM", "");

                                            pstatusdatetime = pstatusdate + " " + pstatustime.ToString();

                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            pdtPODDateTime = Convert.ToDateTime(pstatusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            ptrack_PODStatus = RLDS.Tables["Status"].Rows[i]["Description"].ToString().Trim();
                                            if (ptrack_PODStatus.ToUpper() == "DELIVERED")
                                            {
                                                ptrack_PODStatus = "Delivered";
                                                pstrStatus_code = "DEL";
                                            }
                                            else
                                            {
                                                // ptrack_PODStatus = "In-Transit";
                                                pstrStatus_code = "INT";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ptrack_PODStatus = "";
                                        }

                                        string pstrDatetime = null;
                                        try
                                        {
                                            pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        try
                                        {
                                            ptrack_podLocation = ""; ;

                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        pstrInsertQuery = pstrInsertQuery + "('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'),";

                                    }
                                    if (pstrInsertQuery != "")
                                    {
                                        try
                                        {
                                            pstrInsertQuery = pstrInsertQuery.Substring(0, pstrInsertQuery.Length - 1);

                                            SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                            if (con.State == ConnectionState.Closed)
                                            {
                                                con.Open();
                                            }
                                            SqlCmd.ExecuteNonQuery();

                                            if (con.State == ConnectionState.Open)
                                            {
                                                con.Close();
                                            }

                                            System.Threading.Thread.Sleep(100);
                                        }
                                        catch (Exception ex)
                                        {

                                            if (con.State == ConnectionState.Open)
                                                con.Close();

                                        }
                                    }



                                }
                                else
                                {
                                    strStatus_code = "INT";
                                }
                            }
                            catch (Exception ex)
                            {
                                track_PODStatus = "";
                            }
                            XCARRIERUPDATE(con, dtPODDateTime, "", track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);

                        }

                    }

                }
                catch (Exception ex)
                {
                    addquery("Unable run RL_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
                }

            }
            catch (Exception ex)
            {
                addquery("Unable run RL_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
            }
            try
            {
                if (RLDS != null)
                {
                    RLDS.Dispose();
                }
            }
            catch (Exception ex)
            {


            }
        }



        //  String strDate, string shippingno, string erpenginepath, string ERPUpdateFlag, string LabelPath, DataSet dsCarrier, SqlConnection con, string CustomerID, 
        //   string carrier, string ERPName, string DeliveryNUM, string FeederSystemName, string MastertrackingNo)
        public static void DHL_Status(object pparam_obj)
        {
            string sDHLEuropeUserID = "";
            string sDHLEuropeUserPwd = "";
            string shippingno = "", TrackingNo = "", ERPUpdateFlag = "", ERPName = "", FeederSystemName = "", strShipFromCountry = "", Service = "", MastertrackingNo = "", DeliveryNUM = "", CustomerID = "", strDate = "", AccountNo = "", carrier = "", PlantID = "", connectionstring = "", erpenginepath = "", LabelPath = "";
            SqlConnection con = null;
            try
            {

                try
                {
                    Array DHLargArray = new object[2];
                    DHLargArray = (Array)pparam_obj;

                    AccountNo = (string)DHLargArray.GetValue(0);
                    TrackingNo = (string)DHLargArray.GetValue(1);
                    PlantID = (string)DHLargArray.GetValue(2);
                    Service = (string)DHLargArray.GetValue(3);
                    connectionstring = (string)DHLargArray.GetValue(4);
                    strShipFromCountry = (string)DHLargArray.GetValue(5);
                    strDate = (string)DHLargArray.GetValue(6);
                    shippingno = (string)DHLargArray.GetValue(7);
                    erpenginepath = (string)DHLargArray.GetValue(8);
                    ERPUpdateFlag = (string)DHLargArray.GetValue(9);
                    LabelPath = (string)DHLargArray.GetValue(10);
                    DataSet dsCarrier = (DataSet)DHLargArray.GetValue(11);
                    con = (SqlConnection)DHLargArray.GetValue(12);
                    CustomerID = (string)DHLargArray.GetValue(13);
                    carrier = (string)DHLargArray.GetValue(14);
                    ERPName = (string)DHLargArray.GetValue(15);
                    DeliveryNUM = (string)DHLargArray.GetValue(16);
                    FeederSystemName = (string)DHLargArray.GetValue(17);
                    MastertrackingNo = (string)DHLargArray.GetValue(18);

                    DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number


                    if (dataRows.Length > 0)
                    {
                        sDHLEuropeUserID = dataRows[0]["USER_ID"].ToString();
                        sDHLEuropeUserPwd = dataRows[0]["PASSWORD"].ToString();
                    }

                }
                catch (Exception)
                {

                }


                DateTime dtShipDate = DateTime.Now;
                string strXML = "";
                strXML = "<?xml version='1.0' encoding='UTF-8' ?>";
                strXML = strXML + "<req:KnownTrackingRequest  xmlns:req='http://www.dhl.com' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:schemaLocation='http://www.dhl.com TrackingRequestKnown.xsd'>";
                strXML = strXML + "<Request>";
                strXML = strXML + "<ServiceHeader>";
                strXML = strXML + "<MessageTime>" + dtShipDate.ToString("s") + "</MessageTime>";
                strXML = strXML + "<MessageReference>Esteemed Courier Service of DHL</MessageReference>";
                strXML = strXML + "<SiteID>" + sDHLEuropeUserID + "</SiteID>";
                strXML = strXML + "<Password>" + sDHLEuropeUserPwd + "</Password>";
                strXML = strXML + "</ServiceHeader>";
                strXML = strXML + "</Request>";
                strXML = strXML + "<LanguageCode>en</LanguageCode>";
                strXML = strXML + "<AWBNumber>" + TrackingNo + "</AWBNumber>";
                // strXML = strXML + "<AWBNumber>9687743785</AWBNumber>";
                strXML = strXML + "<LevelOfDetails>ALL_CHECK_POINTS</LevelOfDetails>";
                strXML = strXML + "<PiecesEnabled>S</PiecesEnabled>";
                strXML = strXML + "</req:KnownTrackingRequest>";

                try
                {
                    string mydocpath1 = LogPath;// "C:\\Processweaver\\BackUp";


                    using (System.IO.StreamWriter outfile =
                      new System.IO.StreamWriter(mydocpath1 + @"\DHLTrackRequest.xml"))
                    {
                        outfile.Write(strXML);
                        outfile.Close();
                        outfile.Dispose();
                        //GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                }


                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var req = (HttpWebRequest)WebRequest.Create("https://xmlpi-ea.dhl.com/XMLShippingServlet");

                    req.PreAuthenticate = true;
                    req.AllowAutoRedirect = false;
                    req.Method = "POST";
                    //req.ContentType = "application/xml";
                    req.ContentType = "application/x-www-form-urlencoded";
                    string strxml1 = strXML;
                    System.IO.Stream RequestStreamFDX = req.GetRequestStream();
                    byte[] SomeBytes = Encoding.UTF8.GetBytes(strxml1);
                    RequestStreamFDX.Write(SomeBytes, 0, SomeBytes.Length);
                    RequestStreamFDX.Close();
                    RequestStreamFDX.Dispose();

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                    if (myResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Stream ReceiveStream = myResponse.GetResponseStream();
                        Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                        StreamReader readStream = new StreamReader(ReceiveStream, encode);
                        string stripresponse = readStream.ReadToEnd();

                        stripresponse = stripresponse.Replace("<req:TrackingResponse xmlns:req=\"http://www.dhl.com\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.dhl.com TrackingResponse.xsd\">", "<req>");
                        stripresponse = stripresponse.Replace("</req:TrackingResponse>", "</req>");

                        readStream.Close();
                        readStream.Dispose();



                        try
                        {
                            string mydocpath1 = LogPath; //"C:\\Processweaver\\BackUp";


                            using (System.IO.StreamWriter outfile =
                              new System.IO.StreamWriter(mydocpath1 + @"\DHLTrackResponse.xml"))
                            {
                                outfile.Write(stripresponse);
                                outfile.Close();
                                outfile.Dispose();
                                //GC.Collect();
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        XmlDocument myxmldocument = new XmlDocument();
                        myxmldocument.LoadXml(stripresponse);


                        string severiaty = myxmldocument.GetElementsByTagName("ActionStatus")[0].InnerText;
                        if (severiaty.ToUpper() == "SUCCESS")
                        {

                            System.Xml.XmlNodeList ActivityNodeList = myxmldocument.SelectNodes("/req/AWBInfo/ShipmentInfo/ShipmentEvent");
                            //deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);


                            string track_podsignature = "";
                            string track_podLocation = "";
                            DateTime? dtPODDateTime = null;
                            string track_PODStatus = "";

                            int k = ActivityNodeList.Count - 1;
                            try
                            {

                                track_podsignature = ActivityNodeList[k].SelectSingleNode("Signatory").InnerText;
                            }
                            catch (Exception ex)
                            {
                                track_podsignature = "";
                            }
                            string statusdate = "";
                            try
                            {
                                statusdate = ActivityNodeList[k].SelectSingleNode("Date").InnerText + " " + ActivityNodeList[k].SelectSingleNode("Time").InnerText;

                            }
                            catch (Exception ex)
                            {

                            }


                            try
                            {
                                dtPODDateTime = Convert.ToDateTime(statusdate, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                            }
                            catch (Exception ex)
                            {

                            }
                            string City = "";

                            try
                            {
                                string servicearea = myxmldocument.SelectNodes("/req/AWBInfo/ShipmentInfo/Shipper/City")[0].InnerText;
                                char[] splitchar = { '-' };
                                string[] arrCity = servicearea.Split(splitchar);
                                City = arrCity[0].ToString();

                            }
                            catch (Exception ex)
                            {
                                City = "";

                            }
                            string CountryCode = "";
                            try
                            {

                                CountryCode = myxmldocument.SelectNodes("/req/AWBInfo/ShipmentInfo/Shipper/CountryCode")[0].InnerText;
                                //CountryCode = ActivityNodeList[k].SelectSingleNode("CountryCode").InnerText;
                            }
                            catch (Exception ex)
                            {

                                CountryCode = "";
                            }

                            try
                            {
                                if (City != "" && CountryCode != "")
                                    track_podLocation = City + ", " + CountryCode;
                                else if (CountryCode != "")
                                {
                                    track_podLocation = CountryCode;
                                }
                            }
                            catch (Exception ex)
                            {
                                track_podLocation = "";
                            }

                            try
                            {
                                track_PODStatus = ActivityNodeList[k].SelectSingleNode("ServiceEvent/EventCode").InnerText;
                                if (track_PODStatus == "OK")
                                    track_PODStatus = "DELIVERED";
                            }
                            catch (Exception ex)
                            {
                                track_PODStatus = "";
                            }
                            string strStatus_code = "";
                            string pstrInsertQuery = "";
                            try
                            {
                                //strStatus_code = UPSSTATUS.Tables["StatusCode"].Rows[0]["Code"].ToString();
                                if (track_PODStatus.ToUpper() == "DELIVERED")
                                {
                                    strStatus_code = "DEL";
                                }
                                else
                                {
                                    strStatus_code = "INT";

                                }

                            }
                            catch (Exception ex)
                            {
                                strStatus_code = "";
                            }
                            try
                            {
                                if (track_PODStatus.ToUpper() == "DELIVERED")
                                {
                                    deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);

                                    System.Threading.Thread.Sleep(100);

                                    if (ActivityNodeList.Count > 0)
                                    {
                                        pstrInsertQuery = "insert into [XCARRIER_SHIPPING_VISIBILITY] (shipping_num,POD_DATETIME,POD_SIGN,POD_STATUS,Tracking_num,StatusCode,CURRENT_PLACE,CARRIER) values ";
                                    }

                                    if (ActivityNodeList.Count > 0)
                                    {

                                        for (int d = 0; d < ActivityNodeList.Count; d++)
                                        {
                                            string ptrack_podLocation = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrStatus_code = "", ptrack_podsignature = "";
                                            DateTime? pdtPODDateTime = null;

                                            try
                                            {
                                                // statusdate = ActivityNodeList[k].SelectSingleNode("Date").InnerText + " " + ActivityNodeList[k].SelectSingleNode("Time").InnerText;

                                                pstatusdatetime = ActivityNodeList[d].SelectSingleNode("Date").InnerText + " " + ActivityNodeList[d].SelectSingleNode("Time").InnerText;
                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            try
                                            {
                                                pdtPODDateTime = Convert.ToDateTime(pstatusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            try
                                            {
                                                ptrack_PODStatus = ActivityNodeList[d].SelectSingleNode("ServiceEvent/EventCode").InnerText;

                                                if (ptrack_PODStatus == "OK")
                                                {
                                                    ptrack_PODStatus = "Delivered";
                                                    pstrStatus_code = "DEL";
                                                }
                                                else
                                                {
                                                    ptrack_PODStatus = ActivityNodeList[d].SelectSingleNode("ServiceEvent/Description").InnerText;
                                                    pstrStatus_code = "INT";
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                ptrack_PODStatus = "";
                                            }

                                            string pstrDatetime = null;
                                            try
                                            {
                                                pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                            }
                                            catch (Exception ex)
                                            {


                                            }
                                            try
                                            {
                                                ptrack_podsignature = ActivityNodeList[d].SelectSingleNode("Signatory").InnerText;

                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                ptrack_podLocation = myxmldocument.SelectNodes("/req/AWBInfo/ShipmentInfo/Shipper/City")[0].InnerText;

                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            pstrInsertQuery = pstrInsertQuery + "('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'),";

                                        }
                                        if (pstrInsertQuery != "")
                                        {
                                            try
                                            {
                                                pstrInsertQuery = pstrInsertQuery.Substring(0, pstrInsertQuery.Length - 1);
                                                addquery("DHL pstrInsertQuery :: " + pstrInsertQuery + ":: TrackingNumber" + TrackingNo);

                                                SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                                if (con.State == ConnectionState.Closed)
                                                {
                                                    con.Open();
                                                }
                                                SqlCmd.ExecuteNonQuery();


                                                if (con.State == ConnectionState.Open)
                                                {
                                                    con.Close();
                                                }

                                                System.Threading.Thread.Sleep(100);
                                            }
                                            catch (Exception ex)
                                            {
                                                addquery("DHL Ship Visibility Insert Exception :: TrackingNumber" + TrackingNo + " ::" + ex.ToString());

                                                if (con.State == ConnectionState.Open)
                                                    con.Close();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    track_PODStatus = "INT";
                                }
                            }
                            catch (Exception exp)
                            {

                            }
                            try
                            {

                                addquery("===============DHL XCARRIERUPDATE ============");
                                XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, "0", MastertrackingNo, carrier);

                            }
                            catch (Exception exp)
                            {

                                addquery("===============EXP DHL XCARRIERUPDATE ============" + exp.ToString());
                            }


                            if (ERPUpdateFlag.ToUpper() == "TRUE" && FeederSystemName != "Manual" && k.ToString() == "0")
                            {
                                ERPUPDATE(erpenginepath, PlantID, CustomerID, TrackingNo, track_PODStatus, track_podsignature, dtPODDateTime, ERPName, DeliveryNUM, carrier, shippingno);
                            }

                        }
                    }


                }
                catch (Exception ex)
                {
                    //using (WebResponse response = ex.Response)
                    //{
                    //    //using (Stream data = response.GetResponseStream())
                    //    //using (reader = new StreamReader(data)) { stripresponse = reader.ReadToEnd(); }
                    //}
                    //addquery("Exception from DHL 401" + stripresponse.ToString() + ex.Message.ToString());
                }
                finally
                {

                }
            }
            catch (Exception exp)
            {
                addquery("Unable run DHL_FREIGHT_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + exp.ToString());
            }
        }

        // public static void TNT_Carrier_Status(string AccountNo, string TrackingNo, string PlantID, string Service, string connectionstring, String strShipFromCountry,
        // String strDate, string shippingno, string erpenginepath, string ERPUpdateFlag, string LabelPath, DataSet dsCarrier, SqlConnection con, string CustomerID, 
        //   string carrier, string ERPName, string DeliveryNUM, string FeederSystemName, string MastertrackingNo)
        public static void TNT_Carrier_Status(object pparam_obj)
        {
            string shippingno = "", ERPUpdateFlag = "", TrackingNo = "", ERPName = "", FeederSystemName = "", strShipFromCountry = "", Service = "", MastertrackingNo = "", DeliveryNUM = "", CustomerID = "", strDate = "", AccountNo = "", carrier = "", PlantID = "", connectionstring = "", erpenginepath = "", LabelPath = "";
            SqlConnection con = null;
            try
            {
                Array TNTargArray = new object[2];
                TNTargArray = (Array)pparam_obj;

                AccountNo = (string)TNTargArray.GetValue(0);
                TrackingNo = (string)TNTargArray.GetValue(1);
                PlantID = (string)TNTargArray.GetValue(2);
                Service = (string)TNTargArray.GetValue(3);
                connectionstring = (string)TNTargArray.GetValue(4);
                strShipFromCountry = (string)TNTargArray.GetValue(5);
                strDate = (string)TNTargArray.GetValue(6);
                shippingno = (string)TNTargArray.GetValue(7);
                erpenginepath = (string)TNTargArray.GetValue(8);
                ERPUpdateFlag = (string)TNTargArray.GetValue(9);
                LabelPath = (string)TNTargArray.GetValue(10);
                DataSet dsCarrier = (DataSet)TNTargArray.GetValue(11);
                con = (SqlConnection)TNTargArray.GetValue(12);
                CustomerID = (string)TNTargArray.GetValue(13);
                carrier = (string)TNTargArray.GetValue(14);
                ERPName = (string)TNTargArray.GetValue(15);
                DeliveryNUM = (string)TNTargArray.GetValue(16);
                FeederSystemName = (string)TNTargArray.GetValue(17);
                MastertrackingNo = (string)TNTargArray.GetValue(18);

                string summarycode = "";
                string status_date = "";
                string dateformation = "";
                string ISPOD = "";
                string PODstatus = "";
                string podsign = "";
                string strTNTdatetime = "";

                DataSet TNTDS = new DataSet();
                TNTDS.Clear();
                string result = "", TNTUserid = "", TNTPassword = "";
                StreamReader reader = null;
                string stripresponse = "";



                try
                {
                    SqlDataAdapter da = new SqlDataAdapter("select * from xCarrier_carrier WHERE Account_number ='" + AccountNo + "'", con);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    TNTUserid = ds.Tables[0].Rows[0]["User_id"].ToString();
                    TNTPassword = ds.Tables[0].Rows[0]["Password"].ToString();
                }
                catch (Exception ex)
                {
                    addquery("TNT CARRIER CREDENTIALS:" + ex.Message.ToString());
                }

                try
                {
                    SqlDataAdapter da1 = new SqlDataAdapter("select Country from xCarrier_location WHERE Plant_ID ='" + PlantID + "'", con);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);

                    strShipFromCountry = ds1.Tables[0].Rows[0]["Country"].ToString();
                }
                catch (Exception ex)
                {

                }

                // TrackingNo = "328110580";
                //addquerTNT("ShipFromCountry:"+strShipFromCountry);
                string XML_HEADER = "xml_in=<?xml version='1.0' encoding='utf-8' standalone='no' ?>" + "\n";
                StringBuilder xmlInput = new StringBuilder(XML_HEADER);
                xmlInput.Append("<TrackRequest locale=\"en_US\" version=\"3.1\">" + "\n");
                xmlInput.Append("<SearchCriteria marketType='International' originCountry='" + strShipFromCountry + "'>" + "\n");
                xmlInput.Append("<ConsignmentNumber>" + TrackingNo + "</ConsignmentNumber>" + "\n");

                xmlInput.Append("</SearchCriteria>" + "\n");
                xmlInput.Append("<Account>" + "\n");
                xmlInput.Append("<Number>" + AccountNo + "</Number>" + "\n");

                xmlInput.Append("<CountryCode>" + strShipFromCountry + "</CountryCode>" + "\n");
                xmlInput.Append("</Account>" + "\n");
                xmlInput.Append("<LevelOfDetail>" + "\n");
                xmlInput.Append("<Complete originAddress='true' destinationAddress='true' package='true' shipment='true' />" + "\n");
                xmlInput.Append("<POD format='URL'/>");
                xmlInput.Append("</LevelOfDetail>" + "\n");
                xmlInput.Append("</TrackRequest>" + "\n");
                result = xmlInput.ToString();

                try
                {
                    string mydocpath1 = LogPath; //"C:\\Processweaver\\BackUp";


                    using (System.IO.StreamWriter outfile =
                      new System.IO.StreamWriter(mydocpath1 + @"\TNT_TrackRequest.xml"))
                    {
                        outfile.Write(result);
                        outfile.Close();
                        outfile.Dispose();
                        //GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                }

                ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var req = (HttpWebRequest)WebRequest.Create("https://express.tnt.com/expressconnect/track.do?");
                //req.Credentials = GetCredential("https://express.tnt.com/expressconnect/track.do?version=3", "GELIVESCI", "gE-livesc1");

                req.Credentials = GetCredential("https://express.tnt.com/expressconnect/track.do?", TNTUserid, TNTPassword);
                req.PreAuthenticate = true;
                req.AllowAutoRedirect = false;
                req.Method = "POST";
                //req.ContentType = "application/xml";
                req.ContentType = "application/x-www-form-urlencoded";
                string strxml1 = result;
                System.IO.Stream RequestStreamFDX = req.GetRequestStream();
                byte[] SomeBytes = Encoding.UTF8.GetBytes(strxml1);
                RequestStreamFDX.Write(SomeBytes, 0, SomeBytes.Length);
                RequestStreamFDX.Close();
                using (var resp = (HttpWebResponse)req.GetResponse())
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                    using (Stream data = resp.GetResponseStream())
                    using (reader = new StreamReader(data))
                    {
                        stripresponse = reader.ReadToEnd();

                        string str = "[^\x09\x0A\x0D\x20 -\xD7FF\xE000 -\xFFFD\x10000 - x10FFFF]";

                        stripresponse = Regex.Replace(stripresponse, str, "");

                        XmlDocument myxmldocument = new XmlDocument();
                        myxmldocument.LoadXml(stripresponse);

                        try
                        {
                            string mydocpath1 = LogPath; //"C:\\Processweaver\\BackUp";


                            using (System.IO.StreamWriter outfile =
                              new System.IO.StreamWriter(mydocpath1 + @"\TNT_TrackResponse.xml"))
                            {
                                outfile.Write(stripresponse);
                                outfile.Close();
                                outfile.Dispose();
                                //GC.Collect();
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        try
                        {
                            summarycode = myxmldocument.GetElementsByTagName("SummaryCode")[0].InnerText;
                        }
                        catch (Exception ex)
                        {

                        }
                        if (summarycode == "DEL" || summarycode == "INT" || summarycode == "EXC" || summarycode == "CNF")
                        {
                            TNTDS.ReadXml(new XmlTextReader(new System.IO.StringReader(myxmldocument.InnerXml)));
                            if (TNTDS.Tables.Count == 1)
                            {

                            }
                            else
                            {
                                string track_podsignature = "";
                                string track_podLocation = "";
                                DateTime? dtPODDateTime = null;
                                string track_PODStatus = "";
                                string podDatetime = "", PODCity = "", PODCountry = "";
                                int k = 0;
                                string statusdate = "", statusdatetime = "";
                                try
                                {
                                    try
                                    {
                                        // podsign = TNTDS.Tables["Consignment"].Rows[0]["Signatory"].ToString();
                                        track_podsignature = TNTDS.Tables["Consignment"].Rows[0]["Signatory"].ToString();
                                    }
                                    catch (Exception ex)
                                    {
                                        track_podsignature = "";
                                    }
                                    try
                                    {
                                        podDatetime = TNTDS.Tables["LocalEventTime"].Rows[k]["LocalEventTime_Text"].ToString();
                                        dateformation = TNTDS.Tables["LocalEventDate"].Rows[k]["LocalEventDate_Text"].ToString();
                                        string year = dateformation.Substring(0, 4);
                                        string month = dateformation.Substring(4, 2);
                                        string dat = dateformation.Substring(6, 2);

                                        string hours = podDatetime.Substring(0, 2);
                                        string minutes = podDatetime.Substring(2, 2);
                                        statusdatetime = year + "-" + month + "-" + dat + " " + hours + ":" + minutes + ":00";
                                        dtPODDateTime = Convert.ToDateTime(statusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    try
                                    {
                                        PODCity = TNTDS.Tables["StatusData"].Rows[0]["DepotName"].ToString();
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    string country = "";
                                    try
                                    {
                                        PODCountry = TNTDS.Tables["OriginCountry"].Rows[0]["CountryCode"].ToString();
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    try
                                    {
                                        track_podLocation = country + "," + PODCountry;
                                    }
                                    catch (Exception)
                                    {

                                    }

                                    try
                                    {
                                        track_PODStatus = TNTDS.Tables["StatusData"].Rows[k]["StatusCode"].ToString();
                                        if (track_PODStatus == "OK")
                                        {
                                            track_PODStatus = "DELIVERED";
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        track_PODStatus = "";
                                    }
                                    string strStatus_code = "";
                                    string pstrInsertQuery = "";
                                    if (TNTDS.Tables["StatusData"].Rows.Count > 0)
                                    {
                                        if (track_PODStatus == "DELIVERED")
                                        {
                                            strStatus_code = "DEL";
                                            try
                                            {
                                                string URLImage = TNTDS.Tables["Consignment"].Rows[0]["POD"].ToString();

                                                ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                                                using (WebClient client = new WebClient())
                                                {
                                                    client.DownloadFile(URLImage, LabelPath + "\\" + TrackingNo + ".pdf");

                                                    System.Threading.Thread.Sleep(100);
                                                }
                                                ISPOD = "True";

                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);

                                            System.Threading.Thread.Sleep(100);
                                            if (TNTDS.Tables["StatusData"].Rows.Count > 0)
                                            {
                                                pstrInsertQuery = "insert into [XCARRIER_SHIPPING_VISIBILITY] (shipping_num,POD_DATETIME,POD_SIGN,POD_STATUS,Tracking_num,StatusCode,CURRENT_PLACE,CARRIER) values ";
                                            }

                                            for (int i = 0; i < TNTDS.Tables["StatusData"].Rows.Count; i++)
                                            {
                                                string ptrack_podLocation = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrStatus_code = "", ptrack_podsignature = "";
                                                DateTime? pdtPODDateTime = null;

                                                string pod_time = "";
                                                try
                                                {
                                                    pod_time = TNTDS.Tables["LocalEventTime"].Rows[i]["LocalEventTime_Text"].ToString();
                                                }
                                                catch (Exception ex)
                                                {
                                                    pod_time = "00:00:00";
                                                }

                                                try
                                                {
                                                    dateformation = TNTDS.Tables["LocalEventDate"].Rows[i]["LocalEventDate_Text"].ToString();
                                                    string year = dateformation.Substring(0, 4);
                                                    string month = dateformation.Substring(4, 2);
                                                    string dat = dateformation.Substring(6, 2);

                                                    string hours = pod_time.Substring(0, 2);
                                                    string minutes = pod_time.Substring(2, 2);
                                                    pstatusdatetime = year + "-" + month + "-" + dat + " " + hours + ":" + minutes + ":00";
                                                    pdtPODDateTime = Convert.ToDateTime(pstatusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                                }
                                                catch (Exception ex)
                                                {
                                                    pstatusdatetime = "";
                                                }

                                                string status_code = "";
                                                try
                                                {
                                                    status_code = TNTDS.Tables["StatusData"].Rows[i]["StatusCode"].ToString();
                                                }
                                                catch (Exception ex)
                                                {
                                                    status_code = "";
                                                }
                                                try
                                                {
                                                    ptrack_PODStatus = TNTDS.Tables["StatusData"].Rows[i]["StatusDescription"].ToString();
                                                    if (ptrack_PODStatus == "OK")
                                                    {
                                                        ptrack_PODStatus = "Delivered";
                                                        pstrStatus_code = "DEL";
                                                    }
                                                    else
                                                    {
                                                        pstrStatus_code = "INT";
                                                    }
                                                    string pstrDatetime = null;
                                                    try
                                                    {
                                                        pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    try
                                                    {
                                                        ptrack_podsignature = TNTDS.Tables["Consignment"].Rows[0]["Signatory"].ToString();

                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    try
                                                    {
                                                        ptrack_podLocation = TNTDS.Tables["StatusData"].Rows[i]["DepotName"].ToString() + ", " + TNTDS.Tables["OriginCountry"].Rows[0]["CountryCode"].ToString();

                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    pstrInsertQuery = pstrInsertQuery + "('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'),";



                                                }
                                                catch (Exception ex)
                                                {
                                                    PODstatus = "";
                                                }

                                            }
                                            if (pstrInsertQuery != "")
                                            {
                                                try
                                                {
                                                    pstrInsertQuery = pstrInsertQuery.Substring(0, pstrInsertQuery.Length - 1);
                                                    addquery("TNT pstrInsertQuery :: " + pstrInsertQuery + ":: TrackingNumber" + TrackingNo);
                                                    SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                                    if (con.State == ConnectionState.Closed)
                                                    {
                                                        con.Open();
                                                    }
                                                    SqlCmd.ExecuteNonQuery();
                                                    if (con.State == ConnectionState.Open)
                                                    {
                                                        con.Close();
                                                    }

                                                    System.Threading.Thread.Sleep(100);

                                                }
                                                catch (Exception ex)
                                                {
                                                    addquery("TNT Ship Visibility Insert Exception :: TrackingNumber" + TrackingNo + " ::" + ex.ToString());

                                                    if (con.State == ConnectionState.Open)
                                                        con.Close();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            strStatus_code = "INT";
                                        }
                                        try
                                        {
                                            XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);


                                            if (ERPUpdateFlag.ToUpper() == "TRUE" && FeederSystemName != "Manual" && k.ToString() == "0")
                                            {
                                                ERPUPDATE(erpenginepath, PlantID, CustomerID, TrackingNo, track_PODStatus, track_podsignature, dtPODDateTime, ERPName, DeliveryNUM, carrier, shippingno);
                                            }
                                        }
                                        catch (Exception ex) { }


                                    }

                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }

                    }
                    if (resp.StatusCode != HttpStatusCode.Created)
                    {
                        string message = String.Format("Call failed. Received HTTP {0}", resp.StatusCode);

                    }
                }

            }
            catch (Exception ex)
            {
                addquery("Unable run TNT_Carrier_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
            }

        }

        public static void LOVEEXPRESS_Carrier_Status(object pparam_obj)
        {
            string strLEUserID = "";
            string strLEUserPwd = "";
            string shippingno = "", TrackingNo = "", ERPUpdateFlag = "", ERPName = "", FeederSystemName = "", strShipFromCountry = "", Service = "", MastertrackingNo = "", DeliveryNUM = "", CustomerID = "", strDate = "", AccountNo = "", carrier = "", PlantID = "", connectionstring = "", erpenginepath = "", LabelPath = "";
            SqlConnection con = null;
            try
            {

                try
                {
                    Array LOVEEXPRESSArray = new object[2];
                    LOVEEXPRESSArray = (Array)pparam_obj;

                    AccountNo = (string)LOVEEXPRESSArray.GetValue(0);
                    TrackingNo = (string)LOVEEXPRESSArray.GetValue(1);
                    PlantID = (string)LOVEEXPRESSArray.GetValue(2);
                    Service = (string)LOVEEXPRESSArray.GetValue(3);
                    connectionstring = (string)LOVEEXPRESSArray.GetValue(4);
                    strShipFromCountry = (string)LOVEEXPRESSArray.GetValue(5);
                    strDate = (string)LOVEEXPRESSArray.GetValue(6);
                    shippingno = (string)LOVEEXPRESSArray.GetValue(7);
                    erpenginepath = (string)LOVEEXPRESSArray.GetValue(8);
                    ERPUpdateFlag = (string)LOVEEXPRESSArray.GetValue(9);
                    LabelPath = (string)LOVEEXPRESSArray.GetValue(10);
                    DataSet dsCarrier = (DataSet)LOVEEXPRESSArray.GetValue(11);
                    con = (SqlConnection)LOVEEXPRESSArray.GetValue(12);
                    CustomerID = (string)LOVEEXPRESSArray.GetValue(13);
                    carrier = (string)LOVEEXPRESSArray.GetValue(14);
                    ERPName = (string)LOVEEXPRESSArray.GetValue(15);
                    DeliveryNUM = (string)LOVEEXPRESSArray.GetValue(16);
                    FeederSystemName = (string)LOVEEXPRESSArray.GetValue(17);
                    MastertrackingNo = (string)LOVEEXPRESSArray.GetValue(18);

                    DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number
                    if (dataRows.Length > 0)
                    {
                        strLEUserID = dataRows[0]["USER_ID"].ToString();
                        strLEUserPwd = dataRows[0]["PASSWORD"].ToString();
                    }

                }
                catch (Exception)
                {

                }

                //strLEUserID = "";
                //strLEUserPwd = "";

                //TrackingNo = "";

                string LoveExpressRequest = "";
                LoveExpressRequest = LoveExpressRequest + "" + System.Environment.NewLine;

                LoveExpressRequest = LoveExpressRequest + "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" + System.Environment.NewLine;
                LoveExpressRequest = LoveExpressRequest + "<soap:Header>" + System.Environment.NewLine;
                LoveExpressRequest = LoveExpressRequest + "<AuthHeader xmlns=\"http://tempuri.org/\">" + System.Environment.NewLine;

                LoveExpressRequest = LoveExpressRequest + "<UserName>" + strLEUserID + "</UserName>" + System.Environment.NewLine;
                LoveExpressRequest = LoveExpressRequest + "<Password>" + strLEUserPwd + "</Password>" + System.Environment.NewLine;

                LoveExpressRequest = LoveExpressRequest + "</AuthHeader>" + System.Environment.NewLine;
                LoveExpressRequest = LoveExpressRequest + "</soap:Header>" + System.Environment.NewLine;
                LoveExpressRequest = LoveExpressRequest + "<soap:Body>" + System.Environment.NewLine;
                LoveExpressRequest = LoveExpressRequest + "<GetShipmentsByTrackingNo xmlns=\"http://tempuri.org/\">" + System.Environment.NewLine;
                LoveExpressRequest = LoveExpressRequest + "<Station/>" + System.Environment.NewLine;

                LoveExpressRequest = LoveExpressRequest + "<Housebill>" + TrackingNo + "</Housebill>" + System.Environment.NewLine;

                LoveExpressRequest = LoveExpressRequest + "<Suffix>0</Suffix>" + System.Environment.NewLine;
                LoveExpressRequest = LoveExpressRequest + "</GetShipmentsByTrackingNo>" + System.Environment.NewLine;
                LoveExpressRequest = LoveExpressRequest + "</soap:Body>" + System.Environment.NewLine;
                LoveExpressRequest = LoveExpressRequest + "</soap:Envelope>" + System.Environment.NewLine;

                try
                {
                    string mydocpath1 = LogPath;// "C:\\Processweaver\\BackUp";
                    using (System.IO.StreamWriter outfile =
                      new System.IO.StreamWriter(mydocpath1 + @"\LOVEEXPRESS_TrackRequest.xml"))
                    {
                        outfile.Write(LoveExpressRequest);
                        outfile.Close();
                        outfile.Dispose();
                        //GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                }


                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var req = (HttpWebRequest)WebRequest.Create("https://ssworldtrak.com/WTKServicesLOVE/AirTrakShipment.asmx");

                    req.PreAuthenticate = true;
                    req.AllowAutoRedirect = false;
                    req.Method = "POST";
                    req.ContentType = "text/xml;charset=UTF-8";
                    //req.ContentType = "application/xml";
                    //req.ContentType = "application/soap+xml;charset=UTF-8";
                    string strxml1 = LoveExpressRequest;
                    System.IO.Stream RequestStreamFDX = req.GetRequestStream();
                    byte[] SomeBytes = Encoding.UTF8.GetBytes(strxml1);
                    RequestStreamFDX.Write(SomeBytes, 0, SomeBytes.Length);
                    RequestStreamFDX.Close();
                    RequestStreamFDX.Dispose();

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                    if (myResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Stream ReceiveStream = myResponse.GetResponseStream();
                        Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                        StreamReader readStream = new StreamReader(ReceiveStream, encode);
                        string strResponse = readStream.ReadToEnd();

                        strResponse = strResponse.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
                        //strResponse = strResponse.Replace("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "<Envelope>");
                        strResponse = strResponse.Replace("<soap:Envelope", "<Envelope");

                        strResponse = strResponse.Replace("<soap:Body>", "<Body>");
                        strResponse = strResponse.Replace("</soap:Body>", "</Body>");
                        strResponse = strResponse.Replace("<GetShipmentsByTrackingNoResponse xmlns=\"http://tempuri.org/\">", "<GetShipmentsByTrackingNoResponse>");
                        strResponse = strResponse.Replace("</soap:Envelope>", "</Envelope>");
                        readStream.Close();
                        readStream.Dispose();

                        try
                        {
                            string mydocpath1 = LogPath; //"C:\\Processweaver\\BackUp";
                            using (System.IO.StreamWriter outfile =
                              new System.IO.StreamWriter(mydocpath1 + @"\LOVEEXPRESS_TrackResponse.xml"))
                            {
                                outfile.Write(strResponse);
                                outfile.Close();
                                outfile.Dispose();
                                //GC.Collect();
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        XmlDocument myxmldocument = new XmlDocument();
                        myxmldocument.LoadXml(strResponse);

                        System.Xml.XmlNodeList ActivityNodeList = myxmldocument.SelectNodes("/Envelope/Body/GetShipmentsByTrackingNoResponse/GetShipmentsByTrackingNoResult/ShipmentDetail");

                        string severiaty = "";

                        if (ActivityNodeList.Count > 0)
                        {
                            severiaty = ActivityNodeList[0].SelectSingleNode("ErrorMessage").InnerText;

                            if (severiaty.ToUpper() != "")
                            {

                                //deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);


                                string track_podsignature = "";
                                string track_podLocation = "";
                                DateTime? dtPODDateTime = null;
                                string track_PODStatus = "";

                                int k = ActivityNodeList.Count - 1;
                                try
                                {

                                    track_podsignature = ActivityNodeList[k].SelectSingleNode("ConsigneeName").InnerText;
                                }
                                catch (Exception ex)
                                {
                                    track_podsignature = "";
                                }
                                string statusdate = "";
                                try
                                {
                                    statusdate = ActivityNodeList[k].SelectSingleNode("TrackingNotes/Note/CreatedDate").InnerText.Split('T')[0] + " " + ActivityNodeList[k].SelectSingleNode("TrackingNotes/Note/CreatedTime").InnerText.Split('T')[1];

                                }
                                catch (Exception ex)
                                {

                                }


                                try
                                {
                                    dtPODDateTime = Convert.ToDateTime(statusdate, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                }
                                catch (Exception ex)
                                {

                                }
                                string City = "";

                                //try
                                //{
                                //    string servicearea = myxmldocument.SelectNodes("/req/AWBInfo/ShipmentInfo/Shipper/City")[0].InnerText;
                                //    char[] splitchar = { '-' };
                                //    string[] arrCity = servicearea.Split(splitchar);
                                //    City = arrCity[0].ToString();

                                //}
                                //catch (Exception ex)
                                //{
                                //    City = "";

                                //}

                                City = "";

                                string CountryCode = "";
                                //try
                                //{

                                //    CountryCode = myxmldocument.SelectNodes("/req/AWBInfo/ShipmentInfo/Shipper/CountryCode")[0].InnerText;
                                //    //CountryCode = ActivityNodeList[k].SelectSingleNode("CountryCode").InnerText;
                                //}
                                //catch (Exception ex)
                                //{

                                //    CountryCode = "";
                                //}
                                CountryCode = "";
                                //try
                                //{
                                //    if (City != "" && CountryCode != "")
                                //        track_podLocation = City + ", " + CountryCode;
                                //    else if (CountryCode != "")
                                //    {
                                //        track_podLocation = CountryCode;
                                //    }
                                //}
                                //catch (Exception ex)
                                //{
                                //    track_podLocation = "";
                                //}
                                track_podLocation = "";
                                try
                                {
                                    track_PODStatus = ActivityNodeList[k].SelectSingleNode("Status").InnerText;
                                    if (track_PODStatus == "DEL")
                                        track_PODStatus = "DELIVERED";
                                }
                                catch (Exception ex)
                                {
                                    track_PODStatus = "";
                                }
                                string strStatus_code = "";
                                string pstrInsertQuery = "";
                                try
                                {
                                    //strStatus_code = UPSSTATUS.Tables["StatusCode"].Rows[0]["Code"].ToString();
                                    if (track_PODStatus.ToUpper() == "DELIVERED")
                                    {
                                        strStatus_code = "DEL";
                                    }
                                    else
                                    {
                                        strStatus_code = "INT";

                                    }

                                }
                                catch (Exception ex)
                                {
                                    strStatus_code = "INT";
                                }
                                try
                                {
                                    if (track_PODStatus.ToUpper() == "DELIVERED")
                                    {
                                        deleteSHIPPING_VISIBILITY(con, TrackingNo, shippingno);

                                        System.Threading.Thread.Sleep(100);

                                        if (ActivityNodeList.Count > 0)
                                        {
                                            pstrInsertQuery = "insert into [XCARRIER_SHIPPING_VISIBILITY] (shipping_num,POD_DATETIME,POD_SIGN,POD_STATUS,Tracking_num,StatusCode,CURRENT_PLACE,CARRIER) values ";
                                        }

                                        if (ActivityNodeList.Count > 0)
                                        {

                                            for (int d = 0; d < ActivityNodeList.Count; d++)
                                            {
                                                string ptrack_podLocation = "", pstatusdatetime = "", ptrack_PODStatus = "", pstrStatus_code = "", ptrack_podsignature = "";
                                                DateTime? pdtPODDateTime = null;

                                                try
                                                {
                                                    pstatusdatetime = ActivityNodeList[d].SelectSingleNode("TrackingNotes/Note/CreatedDate").InnerText + " " + ActivityNodeList[d].SelectSingleNode("TrackingNotes/Note/CreatedTime").InnerText;

                                                    //pstatusdatetime = ActivityNodeList[d].SelectSingleNode("Date").InnerText + " " + ActivityNodeList[d].SelectSingleNode("Time").InnerText;
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                                try
                                                {
                                                    pdtPODDateTime = Convert.ToDateTime(pstatusdatetime, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                                try
                                                {
                                                    ptrack_PODStatus = ActivityNodeList[d].SelectSingleNode("Status").InnerText;

                                                    if (ptrack_PODStatus == "DEL")
                                                    {
                                                        ptrack_PODStatus = "Delivered";
                                                        pstrStatus_code = "DEL";
                                                    }
                                                    else
                                                    {
                                                        ptrack_PODStatus = ActivityNodeList[d].SelectSingleNode("Status").InnerText;
                                                        pstrStatus_code = "INT";
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    ptrack_PODStatus = "INT";
                                                }

                                                string pstrDatetime = null;
                                                try
                                                {
                                                    pstrDatetime = pdtPODDateTime?.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) ?? null;
                                                }
                                                catch (Exception ex)
                                                {


                                                }
                                                try
                                                {
                                                    ptrack_podsignature = ActivityNodeList[d].SelectSingleNode("ConsigneeName").InnerText;

                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                                try
                                                {
                                                    ptrack_podLocation = "";
                                                    // ptrack_podLocation = myxmldocument.SelectNodes("/req/AWBInfo/ShipmentInfo/Shipper/City")[0].InnerText;

                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                                pstrInsertQuery = pstrInsertQuery + "('" + shippingno + "','" + pstrDatetime + "','" + ptrack_podsignature.Replace("'", "''") + "','" + ptrack_PODStatus.Replace("'", "''") + "','" + TrackingNo + "','" + pstrStatus_code + "','" + ptrack_podLocation + "','" + carrier + "'),";

                                            }
                                            if (pstrInsertQuery != "")
                                            {
                                                try
                                                {
                                                    pstrInsertQuery = pstrInsertQuery.Substring(0, pstrInsertQuery.Length - 1);
                                                    addquery("LOVEEXPRESS pstrInsertQuery :: " + pstrInsertQuery + ":: TrackingNumber" + TrackingNo);

                                                    SqlCommand SqlCmd = new SqlCommand(pstrInsertQuery, con);
                                                    if (con.State == ConnectionState.Closed)
                                                    {
                                                        con.Open();
                                                    }
                                                    SqlCmd.ExecuteNonQuery();


                                                    if (con.State == ConnectionState.Open)
                                                    {
                                                        con.Close();
                                                    }

                                                    System.Threading.Thread.Sleep(100);
                                                }
                                                catch (Exception ex)
                                                {
                                                    addquery("LOVEEXPRESS Ship Visibility Insert Exception :: TrackingNumber" + TrackingNo + " ::" + ex.ToString());

                                                    if (con.State == ConnectionState.Open)
                                                        con.Close();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        track_PODStatus = "INT";
                                    }
                                }
                                catch (Exception exp)
                                {

                                }
                                try
                                {

                                    addquery("===============LOVEEXPRESS XCARRIERUPDATE ============");
                                    XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, "0", MastertrackingNo, carrier);

                                }
                                catch (Exception exp)
                                {

                                    addquery("===============EXP LOVEEXPRESS XCARRIERUPDATE ============" + exp.ToString());
                                }


                                if (ERPUpdateFlag.ToUpper() == "TRUE" && FeederSystemName != "Manual" && k.ToString() == "0")
                                {
                                    ERPUPDATE(erpenginepath, PlantID, CustomerID, TrackingNo, track_PODStatus, track_podsignature, dtPODDateTime, ERPName, DeliveryNUM, carrier, shippingno);
                                }

                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    //using (WebResponse response = ex.Response)
                    //{
                    //    //using (Stream data = response.GetResponseStream())
                    //    //using (reader = new StreamReader(data)) { stripresponse = reader.ReadToEnd(); }
                    //}
                    //addquery("Exception from LOVEEXPRESS 401" + stripresponse.ToString() + ex.Message.ToString());
                }
                finally
                {

                }
            }
            catch (Exception exp)
            {
                addquery("Unable run LOVEEXPRESS_FREIGHT_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + exp.ToString());
            }
        }

        // public static void COLLISIMO_Status(string AccountNo, string TrackingNo, string PlantID, string Service, string connectionstring, String strShipFromCountry, String strDate, string shippingno, string erpenginepath, string ERPUpdateFlag, string LabelPath, DataSet dsCarrier, SqlConnection con, string CustomerID, string carrier, string ERPName, string DeliveryNUM, string FeederSystemName, string MastertrackingNo, DataSet dsCountryCode)
        public static void COLLISIMO_Status(object pparam_obj)
        {

            try
            {

                string LicenseNo = "";
                string UserId = "";
                string Password = "";
                string _accountno = "";
                string custid = "";


                string strUpdateQuery = string.Empty;
                string strInsertQuery = string.Empty;

                string shippingno = "", ERPUpdateFlag = "", TrackingNo = "", ERPName = "", FeederSystemName = "", strShipFromCountry = "", Service = "", MastertrackingNo = "", DeliveryNUM = "", CustomerID = "", strDate = "", AccountNo = "", carrier = "", PlantID = "", connectionstring = "", erpenginepath = "", LabelPath = "";

                SqlConnection con = null;
                Array COLISSIMOargArray = new object[2];
                COLISSIMOargArray = (Array)pparam_obj;

                AccountNo = (string)COLISSIMOargArray.GetValue(0);
                TrackingNo = (string)COLISSIMOargArray.GetValue(1);
                PlantID = (string)COLISSIMOargArray.GetValue(2);
                Service = (string)COLISSIMOargArray.GetValue(3);
                connectionstring = (string)COLISSIMOargArray.GetValue(4);
                strShipFromCountry = (string)COLISSIMOargArray.GetValue(5);
                strDate = (string)COLISSIMOargArray.GetValue(6);
                shippingno = (string)COLISSIMOargArray.GetValue(7);
                erpenginepath = (string)COLISSIMOargArray.GetValue(8);
                ERPUpdateFlag = (string)COLISSIMOargArray.GetValue(9);
                LabelPath = (string)COLISSIMOargArray.GetValue(10);
                DataSet dsCarrier = (DataSet)COLISSIMOargArray.GetValue(11);
                con = (SqlConnection)COLISSIMOargArray.GetValue(12);
                CustomerID = (string)COLISSIMOargArray.GetValue(13);
                carrier = (string)COLISSIMOargArray.GetValue(14);
                ERPName = (string)COLISSIMOargArray.GetValue(15);
                DeliveryNUM = (string)COLISSIMOargArray.GetValue(16);
                FeederSystemName = (string)COLISSIMOargArray.GetValue(17);
                MastertrackingNo = (string)COLISSIMOargArray.GetValue(18);
                DataSet dsCountryCode = (DataSet)COLISSIMOargArray.GetValue(19);


                XmlDocument MyxmlDocument = new XmlDocument();

                DataRow[] dataRows = dsCarrier.Tables[0].Select("COMPANY_ID = '" + CustomerID + "' and PLANT_ID='" + PlantID + "' and CARRIER_DESCRIPTION='" + carrier + "' and ACCOUNT_NUMBER='" + AccountNo + "'");// to get items by filter package number
                                                                                                                                                                                                                     //double itemweight = 0.0;
                if (dataRows.Length > 0)
                {
                    LicenseNo = dataRows[0]["LICENSE_NUMBER"].ToString();
                    UserId = dataRows[0]["User_ID"].ToString();
                    Password = dataRows[0]["Password"].ToString();
                    _accountno = dataRows[0]["Account_Number"].ToString();
                    custid = dataRows[0]["COMPANY_ID"].ToString();

                }

                //LicenseNo = "FC45B4B21F34AF0C";
                // UserId = "kobayashi0wa";
                //Password = "Ac-19811952";
                //_accountno = "820828";
                //custid = "9243700001";
                string Collismorequest = "";
                string status = "";

                try
                {
                    Collismorequest = "";
                    Collismorequest = Collismorequest + "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:char=\"http://chargeur.tracking.geopost.com/\">" + System.Environment.NewLine;
                    Collismorequest = Collismorequest + "<soapenv:Header/>" + System.Environment.NewLine;
                    Collismorequest = Collismorequest + "<soapenv:Body>" + System.Environment.NewLine;
                    Collismorequest = Collismorequest + "<char:track>" + System.Environment.NewLine;
                    //<!--Optional:-->
                    Collismorequest = Collismorequest + "<accountNumber>" + _accountno + "</accountNumber>" + System.Environment.NewLine;
                    //<!--Optional:-->
                    Collismorequest = Collismorequest + "<password>" + Password + "</password>" + System.Environment.NewLine;
                    //<!--Optional:-->
                    Collismorequest = Collismorequest + "<skybillNumber>" + TrackingNo + "</skybillNumber>" + System.Environment.NewLine;
                    Collismorequest = Collismorequest + "</char:track>" + System.Environment.NewLine;
                    Collismorequest = Collismorequest + "</soapenv:Body>" + System.Environment.NewLine;
                    Collismorequest = Collismorequest + "</soapenv:Envelope>" + System.Environment.NewLine;


                    try
                    {
                        string mydocpath1 = LogPath;// "C:\\Processweaver\\BackUp";


                        using (System.IO.StreamWriter outfile =
                          new System.IO.StreamWriter(mydocpath1 + @"\ColissimoTrackRequest.xml"))
                        {
                            outfile.Write(Collismorequest);
                            outfile.Close();
                            outfile.Dispose();
                            //GC.Collect();
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    HttpWebRequest myRequest1 = (HttpWebRequest)HttpWebRequest.Create("https://www.coliposte.fr/tracking-chargeur-cxf/TrackingServiceWS");
                    myRequest1.AllowAutoRedirect = false;
                    myRequest1.Method = "POST";
                    myRequest1.ContentType = "text/xml;charset=UTF-8";

                    Stream RequestStream1 = myRequest1.GetRequestStream();
                    byte[] SomeBytes1 = Encoding.UTF8.GetBytes(Collismorequest);
                    RequestStream1.Write(SomeBytes1, 0, SomeBytes1.Length);
                    RequestStream1.Close();
                    RequestStream1.Dispose();
                    //ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebResponse myResponse = (HttpWebResponse)myRequest1.GetResponse();
                    if (myResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Stream ReceiveStream = myResponse.GetResponseStream();
                        Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                        StreamReader readStream = new StreamReader(ReceiveStream, encode);
                        string Result = readStream.ReadToEnd();
                        readStream.Close();
                        readStream.Dispose();
                        Result = Result.Replace("soapenv:", "");
                        Result = Result.Replace("common:", "");
                        Result = Result.Replace("trk:", "");
                        Result = Result.Replace("err:", "");
                        try
                        {
                            string mydocpath1 = LogPath; //"C:\\Processweaver\\BackUp";


                            using (System.IO.StreamWriter outfile =
                              new System.IO.StreamWriter(mydocpath1 + @"\ColissimoResponse.xml"))
                            {
                                outfile.Write(Result);
                                outfile.Close();
                                outfile.Dispose();
                                //GC.Collect();
                            }
                        }
                        catch (Exception ex)
                        {
                        }

                        MyxmlDocument.LoadXml(Result);
                        status = MyxmlDocument.GetElementsByTagName("errorCode")[0].InnerText;

                        if (status != "0")
                        {
                            //addquery("Failure message from Colissimo" + MyxmlDocument.GetElementsByTagName("ErrorDescription")[0].InnerText);
                        }
                        else
                        {

                            string track_podsignature = "";
                            string track_podLocation = "";
                            DateTime? dtPODDateTime = null;
                            string track_PODStatus = "";

                            int k = 0;
                            try
                            {
                                track_podsignature = "";
                            }
                            catch (Exception ex)
                            {
                                track_podsignature = "";
                            }

                            string statusdate = "";
                            try
                            {
                                string[] strDateSplit = null;

                                string[] strTimeSplit = null;

                                statusdate = MyxmlDocument.GetElementsByTagName("eventDate")[0].InnerText;
                                strDateSplit = statusdate.Split(new string[] { "T" }, StringSplitOptions.RemoveEmptyEntries);



                                strTimeSplit = strDateSplit[1].Split(new string[] { "+" }, StringSplitOptions.RemoveEmptyEntries);

                                statusdate = strDateSplit[0] + " " + strTimeSplit[0];

                            }
                            catch (Exception ex)
                            {

                            }

                            string upsstatustime = statusdate;
                            try
                            {
                                dtPODDateTime = DateTime.ParseExact(upsstatustime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                            }
                            catch (Exception ex)
                            {

                            }

                            string StateProvinceCode = "";
                            try
                            {
                                StateProvinceCode = "";
                            }
                            catch (Exception ex)
                            {
                                StateProvinceCode = "";
                            }
                            string CountryCode = "";
                            try
                            {
                                CountryCode = MyxmlDocument.GetElementsByTagName("recipientCountryCode")[0].InnerText;
                                if (CountryCode != "")
                                {
                                    DataRow[] dataCCRows = dsCountryCode.Tables[0].Select("COUNTRY_CODE='" + CountryCode + "'");// to get items by filter package number
                                    if (dataCCRows.Length > 0)
                                    {
                                        CountryCode = dataCCRows[0]["COUNTRY"].ToString();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                CountryCode = "";
                            }
                            string City = "";
                            try
                            {
                                City = MyxmlDocument.GetElementsByTagName("recipientCity")[0].InnerText;
                            }
                            catch (Exception ex)
                            {
                                City = "";
                            }
                            try
                            {
                                if (City != "" && CountryCode != "")
                                    track_podLocation = City + ", " + StateProvinceCode + ", " + CountryCode;
                                else if (CountryCode != "")
                                {
                                    track_podLocation = CountryCode;
                                }
                            }
                            catch (Exception ex)
                            {
                                track_podLocation = "";
                            }

                            try
                            {
                                track_PODStatus = MyxmlDocument.GetElementsByTagName("eventLibelle")[0].InnerText;

                            }
                            catch (Exception ex)
                            {
                                track_PODStatus = "";
                            }

                            string strStatus_code = "";

                            try
                            {
                                //strStatus_code = UPSSTATUS.Tables["StatusCode"].Rows[0]["Code"].ToString();
                                if (MyxmlDocument.GetElementsByTagName("eventCode")[0].InnerText.ToUpper() == "LIVCFM")
                                {
                                    strStatus_code = "DEL";
                                    track_PODStatus = "Delivered";
                                }
                                else
                                {
                                    strStatus_code = "INT";

                                }

                            }
                            catch (Exception ex)
                            {
                                strStatus_code = "";
                            }

                            XCARRIERUPDATE(con, dtPODDateTime, track_podsignature, track_PODStatus, track_podLocation, TrackingNo, shippingno, strStatus_code, k.ToString(), MastertrackingNo, carrier);

                            if (ERPUpdateFlag.ToUpper() == "TRUE" && FeederSystemName != "Manual" && status == "0")
                            {
                                ERPUPDATE(erpenginepath, PlantID, CustomerID, TrackingNo, track_PODStatus, track_podsignature, dtPODDateTime, ERPName, DeliveryNUM, carrier, shippingno);
                            }
                            //}

                        }
                    }

                }
                catch (Exception ex)
                {
                    //System.Console.WriteLine("INSIDE UPS POD EXCEPTION: " + ex.Message);
                    addquery("Unable run COLLISIMO_POD for Tracking # :: " + TrackingNo + "   ; Shipping #: " + shippingno + " ; Error Message:: " + ex.ToString());
                }
            }
            catch (Exception)
            {

            }

        }

        public static void addquery(string query)
        {
            try
            {
                string strpath = "";
                strpath = LogPath + "\\" + FolderName + ".txt";
                string FILE_NAME = (strpath);
                System.IO.StreamWriter objWriter = new System.IO.StreamWriter(FILE_NAME, true);

                objWriter.WriteLine(System.Environment.NewLine + "************************************************************************" + System.Environment.NewLine + query);
                objWriter.Close();
                objWriter.Dispose();
            }
            catch (Exception ex)
            {

            }
        }

        private static CredentialCache GetCredential(string url, string username, string password)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            var credentialCache = new CredentialCache();
            credentialCache.Add(new Uri(url), "Basic", new NetworkCredential(username, password));
            return credentialCache;
        }

        public static String getStatusCode(String strStatusCode, String strEventDescription, String strCarrier)
        {
            addquery(strStatusCode + "Carrier Staus Code");
            String strOTMStatusCode = "";
            if (strCarrier.ToUpper() == "FEDEX")
            {
                if (strStatusCode == "PU") // Pick Up.
                    strOTMStatusCode = "CP";
                else if (strStatusCode == "AP") //At Pickup
                    strOTMStatusCode = "RA";
                else if (strStatusCode == "PD") //Pickup Delay
                    strOTMStatusCode = "AW";
                else if (strStatusCode == "PX") // Picked Up.
                    strOTMStatusCode = "CP";
                else if (strStatusCode == "AR") // Picked Up.
                    strOTMStatusCode = "D1";
                else if (strStatusCode == "DL") // Delivered.
                    strOTMStatusCode = "D1";
                else if (strStatusCode == "OD") // Out for Delivery.
                    strOTMStatusCode = "D1";
                else if (strStatusCode == "DE") // Delivery Exception 
                    strOTMStatusCode = "BS";
                else if (strStatusCode == "AD") // At Delivery
                    strOTMStatusCode = "D1";

                else if (strStatusCode == "DY") // Delay
                    strOTMStatusCode = "SD";


                else if (strStatusCode == "CD" && strEventDescription.ToUpper().Contains("DELAY")) // Shipment Delayed.
                    strOTMStatusCode = "SD";
                else if (strStatusCode == "DE" && strEventDescription.ToUpper().Contains("REFUSE")) // Consignee Refused Delivery.
                    strOTMStatusCode = "BG";
                else if (strStatusCode == "DE" && strEventDescription.ToUpper().Contains("CLOSE")) // Consignee Business Closed.
                    strOTMStatusCode = "HC";
                else if (strStatusCode == "DE" && strEventDescription.ToUpper().Contains("DAMAG")) // Shipment Damaged.
                    strOTMStatusCode = "A9";
                else if (strStatusCode == "DE" && strEventDescription.ToUpper().Contains("RETURN")) // Returned to Shipper.
                    strOTMStatusCode = "3";
                else if (strStatusCode == "DP") // Left FedEx origin facility
                    strOTMStatusCode = "BS";

                else if (strStatusCode == "CH") // Location Changed
                    strOTMStatusCode = "BG";
                else if (strStatusCode == "CA") // Shipment Cancelled.
                    strOTMStatusCode = "BG";
                else if (strStatusCode == "RC") // CDO Cancelled.
                    strOTMStatusCode = "BG";
                //else
                //    strOTMStatusCode = "CP";// Pick Up.
            }
            else if (strCarrier.ToUpper() == "UPS")
            {
                if (strStatusCode == "PU") // Pick Up.
                    strOTMStatusCode = "CP";
                else if (strStatusCode == "KB") // Delivered.
                    strOTMStatusCode = "D1";
                else if (strStatusCode == "007") // Shipment Cancelled.
                    strOTMStatusCode = "CA";
                else if (strStatusCode == "046" || strStatusCode == "047" || strStatusCode == "048" || strStatusCode == "049" || strStatusCode == "050" || strStatusCode == "051" || strStatusCode == "052" || strStatusCode == "053") // Shipment Delayed.
                    strOTMStatusCode = "SD";
                else if (strStatusCode == "064") // Consignee Refused Delivery.
                    strOTMStatusCode = "A7";
                else if (strStatusCode == "059") // Shipment Damaged.
                    strOTMStatusCode = "A9";
                else if (strStatusCode == "034" || strStatusCode == "035" || strStatusCode == "036" || strStatusCode == "037" || strStatusCode == "067") // Returned to Shipper.
                    strOTMStatusCode = "3";
                //else
                //    strOTMStatusCode = "CP";// Pick Up.
            }
            else if (strCarrier.ToUpper() == "DHLEUROPE")
            {
                if (strStatusCode == "PU") // Pick Up.
                    strOTMStatusCode = "CP";
                else if (strStatusCode == "DL" || strStatusCode == "DD" || strStatusCode == "DM" || strStatusCode == "OK" || strStatusCode == "NT" || strStatusCode == "MD") // Delivered.
                    strOTMStatusCode = "D1";
                else if (strStatusCode == "CD" || strStatusCode == "MX" || strStatusCode == "TP" || strStatusCode == "UD" || strStatusCode == "WX") // Shipment Delayed.
                    strOTMStatusCode = "SD";
                else if (strStatusCode == "RD") // Consignee Refused Delivery.
                    strOTMStatusCode = "BS";
                else if (strStatusCode == "CA" || strStatusCode == "HX") // Consignee Business Closed.
                    strOTMStatusCode = "HC";
                else if (strStatusCode == "RT") // Returned to Shipper.
                    strOTMStatusCode = "3";
                else if (strStatusCode == "BR")
                    strOTMStatusCode = "C1";
                else if (strStatusCode == "WX") // Weather delay
                    strOTMStatusCode = "SD";
                else if (strStatusCode == "UD") // Clearance delay
                    strOTMStatusCode = "CA";
                else if (strStatusCode == "TD" || strStatusCode == "OH" || strStatusCode == "NA") // Delay in transport to
                    strOTMStatusCode = "SD";
                else if (strStatusCode == "SC") // The requested service for your shipment has been changed; for assistance please contact DHL
                    strOTMStatusCode = "BG";
                else if (strStatusCode == "PM") // A shipment exception has occurred
                    strOTMStatusCode = "AM";
                else if (strStatusCode == "PM") // Partial delivery
                    strOTMStatusCode = "D1";
                else if (strStatusCode == "NH") // Delivery attempted; recipient not home
                    strOTMStatusCode = "BG";
                else if (strStatusCode == "MS") // Shipment arrived at wrong facility. Sent to correct destination
                    strOTMStatusCode = "BG";
                else if (strStatusCode == "MC") // Shipment Arrived at wrong facility. Sent to correct destination
                    strOTMStatusCode = "BG";
                else if (strStatusCode == "MS") // Shipment arrived at wrong facility. Sent to correct destination
                    strOTMStatusCode = "BG";
                else if (strStatusCode == "RT") // Returned to shipper
                    strOTMStatusCode = "BG";
                //else
                //    strOTMStatusCode = "CP";// Pick Up.
            }

            return strOTMStatusCode;
        }

        public static String update_OTM(String strCarrier, String strShipFromCountry, String strTrackingNo, DateTime strPODDate, String strStatus, String strStatusCode, String strExceptionCode, String strPODSignature, String strPODLocation, DateTime dtStartDate, DateTime dtEndDate, String strCity, String strCountry, String strState)
        {
            String Filepath = "C:\\Shipping\\ecs\\OTMConnection.txt";
            //  string Filepath = "C:\\processweaver\\backup\\Connection_2.txt";
            System.IO.StreamReader myFile = new System.IO.StreamReader(Filepath);
            String strconnectionstring = myFile.ReadLine();
            String strOTMURL = myFile.ReadLine();
            myFile.Close();
            String strOTMStatusCode = getStatusCode(strStatusCode, strExceptionCode, strCarrier);
            String strRequest = "";
            if (strOTMStatusCode != "")
            {
                strRequest = strRequest + "<Transmission>" + Environment.NewLine;
                strRequest = strRequest + "<TransmissionHeader>";
                strRequest = strRequest + "<UserName>GECORP.PW_USER</UserName>" + Environment.NewLine;
                strRequest = strRequest + "<Password>CHANGEME</Password>" + Environment.NewLine;
                strRequest = strRequest + "</TransmissionHeader>" + Environment.NewLine;
                strRequest = strRequest + "<TransmissionBody>" + Environment.NewLine;
                strRequest = strRequest + "<GLogXMLElement>" + Environment.NewLine;
                strRequest = strRequest + "<ShipmentStatus>" + Environment.NewLine;
                strRequest = strRequest + "<ServiceProviderAlias>" + Environment.NewLine;
                strRequest = strRequest + "<ServiceProviderAliasQualifierGid>" + Environment.NewLine;
                strRequest = strRequest + "<Gid>" + Environment.NewLine;
                strRequest = strRequest + "<DomainName/>" + Environment.NewLine;
                strRequest = strRequest + "<Xid>GLOG</Xid>" + Environment.NewLine;
                strRequest = strRequest + "</Gid>" + Environment.NewLine;
                strRequest = strRequest + "</ServiceProviderAliasQualifierGid>" + Environment.NewLine;
                strRequest = strRequest + "<ServiceProviderAliasValue>GECORP.106757163</ServiceProviderAliasValue>" + Environment.NewLine;
                //if (strCarrier == "FedEx")
                //    if (strShipFromCountry == "US")
                //        strRequest = strRequest + "<ServiceProviderAliasValue>GECORP.106757163</ServiceProviderAliasValue>" + Environment.NewLine;//Map to actual Carrier ID from PW
                //    else
                //        strRequest = strRequest + "<ServiceProviderAliasValue>GECORP.PARTS_FEDEX_AMS</ServiceProviderAliasValue>" + Environment.NewLine;//Map to actual Carrier ID from PW
                //else if (strCarrier == "UPS")
                //    if (strShipFromCountry == "US")
                //        strRequest = strRequest + "<ServiceProviderAliasValue>GECORP.E36920104</ServiceProviderAliasValue>" + Environment.NewLine;//Map to actual Carrier ID from PW
                //    else
                //        strRequest = strRequest + "<ServiceProviderAliasValue>GECORP.D55267106</ServiceProviderAliasValue>" + Environment.NewLine;//Map to actual Carrier ID from PW
                //else if (strCarrier == "DHLEUROPE")
                //    if (strShipFromCountry == "US")
                //        strRequest = strRequest + "<ServiceProviderAliasValue>GECORP.510173120</ServiceProviderAliasValue>" + Environment.NewLine;//Map to actual Carrier ID from PW
                //    else
                //        strRequest = strRequest + "<ServiceProviderAliasValue>GECORP.696816108</ServiceProviderAliasValue>" + Environment.NewLine;//Map to actual Carrier ID from PW
                //else if (strCarrier == "TNT")
                //    if (strShipFromCountry == "US")
                //        strRequest = strRequest + "<ServiceProviderAliasValue></ServiceProviderAliasValue>" + Environment.NewLine;//Map to actual Carrier ID from PW
                //    else
                //        strRequest = strRequest + "<ServiceProviderAliasValue>GECORP.600293110</ServiceProviderAliasValue>" + Environment.NewLine;//Map to actual Carrier ID from PW
                strRequest = strRequest + "</ServiceProviderAlias>" + Environment.NewLine;
                strRequest = strRequest + "<ShipmentRefnum>" + Environment.NewLine;
                strRequest = strRequest + "<ShipmentRefnumQualifierGid>" + Environment.NewLine;
                strRequest = strRequest + "<Gid>" + Environment.NewLine;
                strRequest = strRequest + "<DomainName/>" + Environment.NewLine;
                strRequest = strRequest + "<Xid>PRO NUMBER</Xid>" + Environment.NewLine;
                strRequest = strRequest + "</Gid>" + Environment.NewLine;
                strRequest = strRequest + "</ShipmentRefnumQualifierGid>" + Environment.NewLine;
                strRequest = strRequest + "<ShipmentRefnumValue>" + strTrackingNo + "</ShipmentRefnumValue>" + Environment.NewLine;
                strRequest = strRequest + "</ShipmentRefnum>" + Environment.NewLine;
                strRequest = strRequest + "<StatusLevel>SHIPMENT</StatusLevel>" + Environment.NewLine;
                strRequest = strRequest + "<StatusCodeGid>" + Environment.NewLine;
                strRequest = strRequest + "<Gid>" + Environment.NewLine;
                strRequest = strRequest + "<DomainName/>" + Environment.NewLine;
                strRequest = strRequest + "<Xid>" + strOTMStatusCode + "</Xid>" + Environment.NewLine;
                strRequest = strRequest + "</Gid>" + Environment.NewLine;
                strRequest = strRequest + "</StatusCodeGid>" + Environment.NewLine;
                strRequest = strRequest + "<EventDt>" + Environment.NewLine;
                strRequest = strRequest + "<GLogDate>" + strPODDate.ToString("yyyyMMddhhmmss") + "</GLogDate>" + Environment.NewLine;
                strRequest = strRequest + "</EventDt>" + Environment.NewLine;
                strRequest = strRequest + "<SSStop>" + Environment.NewLine;
                strRequest = strRequest + "<SSStopSequenceNum>1</SSStopSequenceNum>" + Environment.NewLine;
                if (strOTMStatusCode == "CA" || strOTMStatusCode == "SD" || strOTMStatusCode == "A7" || strOTMStatusCode == "HC" || strOTMStatusCode == "A9" || strOTMStatusCode == "2" || strOTMStatusCode == "3")
                {
                    strRequest = strRequest + "<SSLocation>" + Environment.NewLine;
                    strRequest = strRequest + "<LocationName>" + strCity + "</LocationName>" + Environment.NewLine;
                    strRequest = strRequest + "<EventCity>" + strCity + "</EventCity>" + Environment.NewLine;
                    strRequest = strRequest + "<EventState>" + strState + "</EventState>" + Environment.NewLine;
                    strRequest = strRequest + "<EventCountry>" + getContryCode(strCountry) + "</EventCountry>" + Environment.NewLine;
                    strRequest = strRequest + "<TerminalName/>" + Environment.NewLine;
                    strRequest = strRequest + "</SSLocation>" + Environment.NewLine;

                }
                strRequest = strRequest + "</SSStop>" + Environment.NewLine;
                strRequest = strRequest + "<ResponsiblePartyGid>" + Environment.NewLine;
                strRequest = strRequest + "<Gid>" + Environment.NewLine;
                strRequest = strRequest + "<Xid>CARRIER</Xid>" + Environment.NewLine;
                strRequest = strRequest + "</Gid>" + Environment.NewLine;
                strRequest = strRequest + "</ResponsiblePartyGid>" + Environment.NewLine;
                strRequest = strRequest + "</ShipmentStatus>" + Environment.NewLine;
                strRequest = strRequest + "</GLogXMLElement>" + Environment.NewLine;
                strRequest = strRequest + "<GLogXMLElement>" + Environment.NewLine;
                strRequest = strRequest + "<ActualShipment>" + Environment.NewLine;
                strRequest = strRequest + "<Shipment>" + Environment.NewLine;
                strRequest = strRequest + "<ShipmentHeader>" + Environment.NewLine;
                strRequest = strRequest + "<TransactionCode>IU</TransactionCode>" + Environment.NewLine;
                strRequest = strRequest + "<IntSavedQuery>" + Environment.NewLine;
                strRequest = strRequest + "<IntSavedQueryGid>" + Environment.NewLine;
                strRequest = strRequest + "<Gid>" + Environment.NewLine;
                strRequest = strRequest + "<DomainName>GECORP</DomainName>" + Environment.NewLine;
                strRequest = strRequest + "<Xid>FETCH_SHIPMENT_FROM_PRO_NUMBER</Xid>" + Environment.NewLine;
                strRequest = strRequest + "</Gid>" + Environment.NewLine;
                strRequest = strRequest + "</IntSavedQueryGid>" + Environment.NewLine;
                strRequest = strRequest + "<IntSavedQueryArg>" + Environment.NewLine;
                strRequest = strRequest + "<ArgName>REFNUM_QUAL</ArgName>" + Environment.NewLine;
                strRequest = strRequest + "<ArgValue>PRO NUMBER</ArgValue>" + Environment.NewLine;
                strRequest = strRequest + "</IntSavedQueryArg>" + Environment.NewLine;
                strRequest = strRequest + "<IntSavedQueryArg>" + Environment.NewLine;
                strRequest = strRequest + "<ArgName>REFNUM_VALUE</ArgName>" + Environment.NewLine;
                strRequest = strRequest + "<ArgValue>" + strTrackingNo + "</ArgValue>" + Environment.NewLine;
                strRequest = strRequest + "</IntSavedQueryArg>" + Environment.NewLine;
                strRequest = strRequest + "</IntSavedQuery>" + Environment.NewLine;
                strRequest = strRequest + "<ShipmentRefnum>" + Environment.NewLine;
                strRequest = strRequest + "<ShipmentRefnumQualifierGid>" + Environment.NewLine;
                strRequest = strRequest + "<Gid>" + Environment.NewLine;
                strRequest = strRequest + "<DomainName>GECORP</DomainName>" + Environment.NewLine;
                strRequest = strRequest + "<Xid>POD_SIGNOR</Xid>" + Environment.NewLine;
                strRequest = strRequest + "</Gid>" + Environment.NewLine;
                strRequest = strRequest + "</ShipmentRefnumQualifierGid>" + Environment.NewLine;
                // strRequest = strRequest + "<ShipmentRefnumValue>" + strPODSignature + "</ShipmentRefnumValue>" + Environment.NewLine;
                strRequest = strRequest + "<ShipmentRefnumValue>" + strPODDate.ToString("yyyyMMddhhmmss") + "</ShipmentRefnumValue>" + Environment.NewLine;
                strRequest = strRequest + "</ShipmentRefnum>" + Environment.NewLine;
                strRequest = strRequest + "<ShipmentRefnum>" + Environment.NewLine;
                strRequest = strRequest + "<ShipmentRefnumQualifierGid>" + Environment.NewLine;
                strRequest = strRequest + "<Gid>" + Environment.NewLine;
                strRequest = strRequest + "<DomainName>GECORP</DomainName>" + Environment.NewLine;
                strRequest = strRequest + "<Xid>POD_SIGNED_DATE</Xid>" + Environment.NewLine;
                strRequest = strRequest + "</Gid>" + Environment.NewLine;
                strRequest = strRequest + "</ShipmentRefnumQualifierGid>" + Environment.NewLine;
                strRequest = strRequest + "<ShipmentRefnumValue>" + strPODDate.ToString("yyyyMMddhhmmss") + "</ShipmentRefnumValue>" + Environment.NewLine;
                strRequest = strRequest + "</ShipmentRefnum>" + Environment.NewLine;
                strRequest = strRequest + "<IsServiceTimeFixed>N</IsServiceTimeFixed>";
                strRequest = strRequest + "<StartDt>";
                strRequest = strRequest + "<GLogDate>" + dtStartDate.ToString("yyyyMMddhhmmss") + "</GLogDate>";
                strRequest = strRequest + "</StartDt>";
                strRequest = strRequest + "<EndDt>";
                strRequest = strRequest + "<GLogDate>" + dtEndDate.ToString("yyyyMMddhhmmss") + "</GLogDate>";
                strRequest = strRequest + "</EndDt>";
                strRequest = strRequest + "</ShipmentHeader>" + Environment.NewLine;

                strRequest = strRequest + "<ShipmentStop>" + Environment.NewLine;
                strRequest = strRequest + "<StopSequence>1</StopSequence>" + Environment.NewLine;
                strRequest = strRequest + "<TransactionCode>U</TransactionCode>" + Environment.NewLine;
                strRequest = strRequest + "<ArrivalTime>" + Environment.NewLine;
                strRequest = strRequest + "<EventTime>" + Environment.NewLine;
                strRequest = strRequest + "<EstimatedTime>" + Environment.NewLine;
                strRequest = strRequest + "<GLogDate>" + dtStartDate.ToString("yyyyMMddhhmmss") + "</GLogDate>" + Environment.NewLine;
                strRequest = strRequest + "</EstimatedTime>" + Environment.NewLine;
                strRequest = strRequest + "<IsPlannedTimeFixed>N</IsPlannedTimeFixed>" + Environment.NewLine;
                strRequest = strRequest + "</EventTime>" + Environment.NewLine;
                strRequest = strRequest + "</ArrivalTime>" + Environment.NewLine;
                strRequest = strRequest + "<DepartureTime>" + Environment.NewLine;
                strRequest = strRequest + "<EventTime>" + Environment.NewLine;
                strRequest = strRequest + "<EstimatedTime>" + Environment.NewLine;
                strRequest = strRequest + "<GLogDate>" + dtStartDate.ToString("yyyyMMddhhmmss") + "</GLogDate>" + Environment.NewLine;
                strRequest = strRequest + "</EstimatedTime>" + Environment.NewLine;
                strRequest = strRequest + "<IsPlannedTimeFixed>N</IsPlannedTimeFixed>" + Environment.NewLine;
                strRequest = strRequest + "</EventTime>" + Environment.NewLine;
                strRequest = strRequest + "</DepartureTime>" + Environment.NewLine;
                strRequest = strRequest + "</ShipmentStop>" + Environment.NewLine;
                strRequest = strRequest + "<ShipmentStop>" + Environment.NewLine;
                strRequest = strRequest + "<StopSequence>2</StopSequence>" + Environment.NewLine;
                strRequest = strRequest + "<TransactionCode>U</TransactionCode>" + Environment.NewLine;
                strRequest = strRequest + "<ArrivalTime>" + Environment.NewLine;
                strRequest = strRequest + "<EventTime>" + Environment.NewLine;
                strRequest = strRequest + "<EstimatedTime>" + Environment.NewLine;
                strRequest = strRequest + "<GLogDate>" + dtEndDate.ToString("yyyyMMddhhmmss") + "</GLogDate>" + Environment.NewLine;
                strRequest = strRequest + "</EstimatedTime>" + Environment.NewLine;
                strRequest = strRequest + "<IsPlannedTimeFixed>N</IsPlannedTimeFixed>" + Environment.NewLine;
                strRequest = strRequest + "</EventTime>" + Environment.NewLine;
                strRequest = strRequest + "</ArrivalTime>" + Environment.NewLine;
                strRequest = strRequest + "<DepartureTime>" + Environment.NewLine;
                strRequest = strRequest + "<EventTime>" + Environment.NewLine;
                strRequest = strRequest + "<EstimatedTime>" + Environment.NewLine;
                strRequest = strRequest + "<GLogDate>" + dtEndDate.ToString("yyyyMMddhhmmss") + "</GLogDate>" + Environment.NewLine;
                strRequest = strRequest + "</EstimatedTime>" + Environment.NewLine;
                strRequest = strRequest + "<IsPlannedTimeFixed>N</IsPlannedTimeFixed>" + Environment.NewLine;
                strRequest = strRequest + "</EventTime>" + Environment.NewLine;
                strRequest = strRequest + "</DepartureTime>" + Environment.NewLine;
                strRequest = strRequest + "</ShipmentStop>" + Environment.NewLine;
                strRequest = strRequest + "</Shipment>" + Environment.NewLine;
                strRequest = strRequest + "</ActualShipment>" + Environment.NewLine;
                strRequest = strRequest + "</GLogXMLElement>" + Environment.NewLine;
                strRequest = strRequest + "</TransmissionBody>" + Environment.NewLine;
                strRequest = strRequest + "</Transmission>" + Environment.NewLine;
                addquery(strRequest);
                try
                {
                    HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(strOTMURL);
                    myRequest.AllowAutoRedirect = false;
                    myRequest.Method = "POST";
                    myRequest.ContentType = "text/xml";
                    //Create post stream
                    Stream RequestStream = myRequest.GetRequestStream();
                    byte[] SomeBytes = Encoding.UTF8.GetBytes(strRequest);
                    RequestStream.Write(SomeBytes, 0, SomeBytes.Length);
                    RequestStream.Close();
                    //Send request and get response

                    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                    if (myResponse.StatusCode == HttpStatusCode.OK)
                    {
                        //Get the stream.
                        Stream ReceiveStream = myResponse.GetResponseStream();
                        Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                        //send the stream to a reader. 
                        StreamReader readStream = new StreamReader(ReceiveStream, encode);
                        //Read the result
                        String Status = readStream.ReadToEnd();

                        addquery(Status);
                        return Status;

                    }
                    else
                    {
                        return "";
                    }
                }
                catch (System.Net.WebException webExcp)
                {
                    addquery(webExcp.Message);
                }
            }
            return "";
        }
        public static string CheckFlag(string TrackingNo, string connectionstring)
        {
            SqlConnection con = new SqlConnection(connectionstring);
            con.Open();
            String flag = "";
            try
            {
                SqlDataAdapter da1 = new SqlDataAdapter("Select PickupFlag from packing where TrackingNo='" + TrackingNo + "'", con);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    if (ds1.Tables[0].Rows[0]["PickupFlag"].ToString() == "true")
                    {
                        flag = "true";
                    }
                    else
                    {
                        flag = "false";
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                con.Close();
                con.Dispose();
            }
            return flag;
        }
    }
}
