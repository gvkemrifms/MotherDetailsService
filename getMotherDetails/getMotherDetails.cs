using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace getMotherDetails
{
    public partial class getMotherDetails : ServiceBase
    {
        HelperClass helper = new HelperClass();
        MothersDetails motherDetails = new MothersDetails();
        //  string connectionString = "server=localhost;userid=root;password=1234;database=emri;Convert Zero Datetime=True";
        //string connectionString = "server=10.11.0.16;userid=emri;password=emri;database=emri;Convert Zero Datetime=True";
        //string dateformat = "MM/dd/yyyy HH:mm:ss";
        //string defDate = "1900-01-01 00:00";
        Timer timer = new Timer();
        public getMotherDetails()
        {
            try
            {
                InitializeComponent();
            }
            catch(Exception ex)
            {
                helper.TraceService_abnormal("InitializeComponent -- " + ex.ToString());
            }
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                helper.TraceService("Another entry at :::" + DateTime.Now + "::OnElapsedTime::" + ":::ITS NOT EXCEPTION");
                int hourMinute = Convert.ToInt32(DateTime.Now.ToString("HH"));
                if (hourMinute > 4)
                {
                    DataTable dt = helper.executeSelectStmt("select * from wssyncstatus where isactive = 1;");
                    if (dt.Rows.Count > 0)
                    {
                        string status = dt.Rows[0]["status"].ToString();
                        if (status == "Stopped")
                        {
                            DateTime dtCheck = Convert.ToDateTime(dt.Rows[0]["date"].ToString());
                            double diff2 = Convert.ToDouble((DateTime.Now - dtCheck).TotalDays.ToString());

                            if (diff2 > 0.99)
                            {
                                motherDetails.GetMotherData(dt);
                            }
                            else
                            {
                                string updatestmt = "update wssyncstatus set  lastcheckdate = now(),remarks= 'Data is already processed for: " + DateTime.Now.ToString("yyyy-MM-dd") + "' where isactive=1;";
                                helper.executeInsertStatement(updatestmt);
                            }


                        }
                        else
                        {
                            string updatestmt = "update wssyncstatus set status ='Processing', lastcheckdate = now() where isactive=1;";
                            helper.executeInsertStatement(updatestmt);
                        }
                    }
                }
                else
                {
                    string updatestmt = "update wssyncstatus set lastcheckdate = now() where isactive=1;";
                    helper.executeInsertStatement(updatestmt);
                }
            }
            catch (Exception ex)
            {
                helper.TraceService_abnormal("OnElapsedTime -- " + ex.ToString());
            }


        }


        protected override void OnStart(string[] args)
        {
            try
            {
                helper.traceService("start service");
                timer.Interval = TimeSpan.FromHours(12).TotalMinutes;
                timer.Enabled = true;
                timer.Start();
                timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
               
                //      traceService("Delay time: " + dtDelay.Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                helper.TraceService(ex.ToString() + "~~~~OnStart~~~~" + System.DateTime.Now.ToString());
            }
        }
 
        //private void GetMotherData(DataTable dtWsSyncDetails)
        //{
        //    if (dtWsSyncDetails.Rows[0]["GetMotherData"].ToString() == "1")
        //    {
        //        try
        //        {
        //            GVK_RegistrationDetails._102Integration dt = new GVK_RegistrationDetails._102Integration();
        //            string data = dt.Get_RegistrationDetails("KCRKIT", "102@KCRKIT");
        //            string updatestmt = "update wssyncstatus set status ='Processing', lastcheckdate = now(),currentStatus = 'GetMotherData' where isactive=1;";
        //            helper.executeInsertStatement(updatestmt);
        //            TraceService_result(data);
        //            DataTable dta = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));
        //            //  if( 
        //            //  int count = dta.Rows.Count;

        //            if (dta == null || dta.Rows.Count <= 0)
        //            {

        //                TraceService_abnormal("GETMOTHERDETAILS -- Count is less than or equal to zero");
        //                childDetails.GetChildDetails(dtWsSyncDetails);
        //                // update with 0 - Update_RegistrationDetails(Param1 UserId,Param2 Password,Param3 batchid,Param4 Synchdate,Param4 Response)  with 0
        //            }
        //            else
        //            {
        //                int count = dta.Rows.Count;
        //                int count2 = Convert.ToInt32(dta.Rows[count - 1]["Count"].ToString());
        //                if (count2 == 0)
        //                {
        //                    TraceService_abnormal("GETMOTHERDETAILS -- Count is or equal to zero");
        //                    childDetails.GetChildDetails(dtWsSyncDetails);
        //                }
        //                if (count == count2 + 1)
        //                {
        //                    // Log
        //                    TraceService("GETMOTHERDETAILS -- counts matched for batch id:" + dta.Rows[count - 1]["BatchId"].ToString());
        //                    // insert data
        //                    helper.InsertUpdateMotherData(dta, count);
        //                    GVK_UPDATE_SyncDetails._102Integration US = new GVK_UPDATE_SyncDetails._102Integration();
        //                    string res = US.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "1");
        //                    TraceService("Response after updation for batch id :::" + dta.Rows[count - 1]["BatchId"].ToString() + ":::" + res);
        //                    //
        //                }
        //                else
        //                {
        //                    TraceService_abnormal("Counts are not matched for the batch with batch id " + dta.Rows[count - 1]["BatchId"].ToString());
        //                    GVK_UPDATE_SyncDetails._102Integration US = new GVK_UPDATE_SyncDetails._102Integration();
        //                    string res = US.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), System.DateTime.Now.ToString(), "0");
        //                    TraceService("Response after updation for failed batch id :::" + dta.Rows[count - 1]["BatchId"].ToString() + ":::" + res);

        //                }
        //                GetMotherData(dtWsSyncDetails);
        //            }


        //        }
        //        catch (Exception ex)
        //        {
        //            TraceService_abnormal(ex.ToString());
        //        }
        //    }
        //    else
        //    {
        //        childDetails.GetChildDetails(dtWsSyncDetails);
        //    }
        //}
        //private void GetChildDetails(DataTable dtWsSyncDetails)
        //{
        //    if (dtWsSyncDetails.Rows[0]["GetChildDetails"].ToString() == "1")
        //    {
        //        try
        //        {
        //            GVK_ChildDetails._102Integration dt = new GVK_ChildDetails._102Integration();

        //            string data = dt.Get_ChildDetails("KCRKIT", "102@KCRKIT");
        //            string updatestmt = "update wssyncstatus set status ='Processing', lastcheckdate = now(),currentStatus = 'GetChildDetails' where isactive=1;";
        //            executeInsertStatement(updatestmt);
        //            TraceService_result(data);
        //            DataTable dta = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));

        //            if (dta == null || dta.Rows.Count <= 0)
        //            {
        //                TraceService_abnormal("GetChildDetails is less than or equal to zero");
        //                GetPregancyDetails(dtWsSyncDetails);
        //            }
        //            else
        //            {
        //                int count = dta.Rows.Count;
        //                int count2 = Convert.ToInt32(dta.Rows[count - 1]["Count"].ToString());
        //                if (count2 == 0)
        //                {
        //                    TraceService_abnormal("GETChildDETAILS -- Count is or equal to zero");
        //                    GetPregancyDetails(dtWsSyncDetails);
        //                }
        //                if (count == count2 + 1)
        //                {
        //                    TraceService("GetChildDetails -- counts matched for batch id:" + dta.Rows[count - 1]["BatchId"].ToString());
        //                    InsertUpdateChildDetails(dta, count);
        //                    GVK_UPDATE_SyncDetails._102Integration US = new GVK_UPDATE_SyncDetails._102Integration();
        //                    string res = US.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "1");
        //                    TraceService("Response after updation for batch id :::" + dta.Rows[count - 1]["BatchId"].ToString() + ":::" + res);
        //                    // 
        //                }
        //                else
        //                {
        //                    TraceService_abnormal("Counts are not matched for the batch with batch id " + dta.Rows[count - 1]["BatchId"].ToString());
        //                    GVK_UPDATE_SyncDetails._102Integration US = new GVK_UPDATE_SyncDetails._102Integration();
        //                    string res = US.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), System.DateTime.Now.ToString(), "0");
        //                    TraceService("Response after updation for failed batch id :::" + dta.Rows[count - 1]["BatchId"].ToString() + ":::" + res);

        //                }
        //                GetChildDetails(dtWsSyncDetails);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            TraceService_abnormal(ex.ToString());
        //        }
        //    }
        //    else
        //    {
        //        GetPregancyDetails(dtWsSyncDetails);
        //    }
        //}
        //private void GetPregancyDetails(DataTable dtWsSyncDetails)
        //{
        //    if (dtWsSyncDetails.Rows[0]["GetPregancyDetails"].ToString() == "1")
        //    {
        //        try
        //        {
        //            GVK_PregancyDetails._102Integration dt = new GVK_PregancyDetails._102Integration();
        //            string data = dt.Get_PregancyDetails("KCRKIT", "102@KCRKIT");
        //            TraceService_result(data);
        //            string updatestmt = "update wssyncstatus set status ='Processing', lastcheckdate = now(),currentStatus = 'GetPregancyDetails' where isactive=1;";
        //            executeInsertStatement(updatestmt);
        //            DataTable dta = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));
        //            if (dta == null || dta.Rows.Count <= 0)
        //            {
        //                TraceService_abnormal("GetPregancyDetails -- Count is less than or equal to zero");
        //                GetANCDetails(dtWsSyncDetails);
        //            }
        //            else
        //            {
        //                int count = dta.Rows.Count;
        //                int count2 = Convert.ToInt32(dta.Rows[count - 1]["Count"].ToString());

        //                if (count2 == 0)
        //                {
        //                    TraceService_abnormal("GetPregnancyDetails -- Count is or equal to zero");
        //                    GetANCDetails(dtWsSyncDetails);
        //                }
        //                if (count == count2 + 1)
        //                {
        //                    TraceService("GetPregancyDetails -- counts matched for batch id:" + dta.Rows[count - 1]["BatchId"].ToString());
        //                    InsertUpdatePregencyDetails(dta, count);
        //                    GVK_UPDATE_SyncDetails._102Integration US = new GVK_UPDATE_SyncDetails._102Integration();
        //                    string res = US.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "1");
        //                    TraceService("Response after updation for batch id :::" + dta.Rows[count - 1]["BatchId"].ToString() + ":::" + res);

        //                }
        //                else
        //                {
        //                    TraceService_abnormal("Counts are not matched for the batch with batch id " + dta.Rows[count - 1]["BatchId"].ToString());
        //                    GVK_UPDATE_SyncDetails._102Integration US = new GVK_UPDATE_SyncDetails._102Integration();
        //                    string res = US.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), System.DateTime.Now.ToString(), "0");
        //                    TraceService("Response after updation for failed batch id :::" + dta.Rows[count - 1]["BatchId"].ToString() + ":::" + res);

        //                }
        //                GetPregancyDetails(dtWsSyncDetails);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            TraceService_abnormal(ex.ToString());
        //        }
        //    }
        //    else
        //    {
        //        GetANCDetails(dtWsSyncDetails);
        //    }
        //}
        //private void GetANCDetails(DataTable dtWsSyncDetails)
        //{
        //    if (dtWsSyncDetails.Rows[0]["GetANCDetails"].ToString() == "1")
        //    {
        //        try
        //        {
        //            GVK_ANCDetails._102Integration dt = new GVK_ANCDetails._102Integration();
        //            string data = dt.Get_ANCDetails("KCRKIT", "102@KCRKIT");
        //            string updatestmt = "update wssyncstatus set status ='Processing', lastcheckdate = now(),currentStatus = 'GetANCDetails' where isactive=1;";
        //            executeInsertStatement(updatestmt);
        //            TraceService_result(data);
        //            DataTable dta = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));
        //            if (dta == null || dta.Rows.Count <= 0)
        //            {
        //                TraceService_abnormal("GetANCDetails --Count is less than or equal to zero");
        //                pushPostEventDetails(dtWsSyncDetails);
        //            }
        //            else
        //            {
        //                int count = dta.Rows.Count;
        //                int count2 = Convert.ToInt32(dta.Rows[count - 1]["Count"].ToString());
        //                if (count2 == 0)
        //                {
        //                    TraceService_abnormal("GetANCDetails -- Count is or equal to zero");
        //                    pushPostEventDetails(dtWsSyncDetails);
        //                }
        //                if (count == count2 + 1)
        //                {
        //                    TraceService("counts matched for batch id:" + dta.Rows[count - 1]["BatchId"].ToString());
        //                    InsertUpdateANCDetails(dta, count);
        //                    GVK_UPDATE_SyncDetails._102Integration US = new GVK_UPDATE_SyncDetails._102Integration();
        //                    string res = US.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "1");
        //                    TraceService("Response after updation for batch id :::" + dta.Rows[count - 1]["BatchId"].ToString() + ":::" + res);
        //                }
        //                else
        //                {
        //                    TraceService_abnormal("Counts are not matched for the batch with batch id " + dta.Rows[count - 1]["BatchId"].ToString());
        //                    GVK_UPDATE_SyncDetails._102Integration US = new GVK_UPDATE_SyncDetails._102Integration();
        //                    string res = US.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), System.DateTime.Now.ToString(), "0");
        //                    TraceService("Response after updation for failed batch id :::" + dta.Rows[count - 1]["BatchId"].ToString() + ":::" + res);
        //                }
        //                GetANCDetails(dtWsSyncDetails);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            TraceService_abnormal(ex.ToString());
        //        }
        //    }
        //    else
        //    {
        //        pushPostEventDetails(dtWsSyncDetails);
        //    }
        //}
        //private void InsertUpdateANCDetails(DataTable dta, int count)
        //{
        //    try
        //    {
        //        for (int i = 0; i < count - 1; i++)
        //        {
        //            try
        //            {
        //                DateTime CheckupDate, CreatedDate;
        //                CreatedDate = CheckupDate = Convert.ToDateTime(defDate);
        //                try { CheckupDate = Convert.ToDateTime(dta.Rows[i]["CheckupDate"].ToString()); } catch { }
        //                try { CreatedDate = Convert.ToDateTime(dta.Rows[i]["CreatedDate"].ToString()); } catch { }

        //                MySqlConnection objMySqlConnection = null;
        //                objMySqlConnection = new MySqlConnection(connectionString);
        //                objMySqlConnection.Open();
        //                using (MySqlCommand cmd = objMySqlConnection.CreateCommand())
        //                {
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.CommandText = "Insert_Update_ANCDetails";
        //                    cmd.Parameters.AddWithValue("@Mother_ID", dta.Rows[i]["MotherID"].ToString());
        //                    cmd.Parameters.AddWithValue("@Pregnancy_ID", dta.Rows[i]["PregnancyID"].ToString());
        //                    cmd.Parameters.AddWithValue("@ANC_ID", dta.Rows[i]["ANCID"].ToString());
        //                    cmd.Parameters.AddWithValue("@Checkup_Date", CheckupDate.ToString("yyyy-MM-dd HH:mm:ss"));
        //                    cmd.Parameters.AddWithValue("@Place_Type", dta.Rows[i]["PlaceType"].ToString());
        //                    cmd.Parameters.AddWithValue("@Checkup_Place", dta.Rows[i]["CheckupPlace"].ToString());
        //                    cmd.Parameters.AddWithValue("@Place_Name", dta.Rows[i]["PlaceName"].ToString());
        //                    cmd.Parameters.AddWithValue("@ANC_by", dta.Rows[i]["ANCby"].ToString());
        //                    cmd.Parameters.AddWithValue("@Batch_Id", dta.Rows[count - 1]["BatchId"].ToString());
        //                    cmd.Parameters.AddWithValue("@Type_id", dta.Rows[count - 1]["Typeid"].ToString());
        //                    cmd.Parameters.AddWithValue("@Created_Date", CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
        //                    cmd.ExecuteNonQuery();
        //                }
        //                objMySqlConnection.Close();

        //            }
        //            catch (Exception ex)
        //            {
        //                TraceService_abnormal(ex.ToString());
        //            }

        //        }
        //        string batchselect = "insert into t_batchDetails(`batchid`,`Typeid`,`Count`,`Response`,`Createddate`,`Createdby`,`Updateddate`,`Updatedby`,`Isactive`) values ";
        //        batchselect = batchselect + "( '" + dta.Rows[count - 1]["batchid"].ToString() + "','" + dta.Rows[count - 1]["Typeid"].ToString() + "','" + dta.Rows[count - 1]["Count"].ToString() + "','" + dta.Rows[count - 1]["Response"].ToString() + "', ";
        //        batchselect = batchselect + " '" + dta.Rows[count - 1]["Createddate"].ToString() + "','" + dta.Rows[count - 1]["Createdby"].ToString() + "', ";
        //        batchselect = batchselect + "'" + dta.Rows[count - 1]["Updateddate"].ToString() + "','" + dta.Rows[count - 1]["Updatedby"].ToString() + "','" + dta.Rows[count - 1]["Isactive"].ToString() + "')";
        //        executeInsertStatement(batchselect);
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceService_abnormal(ex.ToString());
        //    }
        //}
        //private void pushPostEventDetails(DataTable dtWsSyncDetails)
        //{
        //    if (dtWsSyncDetails.Rows[0]["pushPostEventDetails"].ToString() == "1")
        //    {
        //        try
        //        {
        //            DataTable dtData = new DataTable();
        //            string cmd = " SELECT bcd.beneficiary_id beneficiary_id, pd.pregnancyid pregnancyid, IFNULL(cdr.childid, '') childid, ";
        //            cmd = cmd + " bcd.`EventTypeId` EventTypeId,bcd.`EventSubTypeId` EventSubTypeId, DATE_FORMAT(bcd.pickup_time, '%m/%d/%Y %H:%i')  pickup_time, ";
        //            cmd = cmd + "DATE_FORMAT(bcd.drop_reach_time, '%m/%d/%Y %H:%i')  drop_reach_time, bcd.DistrictID DistrictID, bcd.dest_location_hosp_id dest_location_hosp_id, bcd.IsAdharverified IsAdharverified, ";
        //            cmd = cmd + " va.vehicle_no vehicle_no FROM beneficiary_case_details bcd JOIN pregnancydetails pd ON pd.`MotherID` = bcd.beneficiary_id ";
        //            cmd = cmd + " JOIN vehicle_assignment va ON va.`TripId` = bcd.`TripId` ";
        //            cmd = cmd + " LEFT JOIN childregistration cdr ON cdr.`MotherID` = bcd.`beneficiary_id` LEFT JOIN t_posteventDetails pe ON pe.`MotherID` = bcd.beneficiary_id WHERE pe.MotherID IS NULL OR pe.status = 'False' limit 0,100";
        //            dtData = executeSelectStmt(cmd);


        //            if (dtData.Rows.Count > 0)
        //            {
        //                string updatestmt = "update wssyncstatus set status ='Processing', lastcheckdate = now(),currentStatus = 'pushPostEventDetails' where isactive=1;";
        //                executeInsertStatement(updatestmt);

        //                foreach (DataRow row in dtData.Rows)
        //                {
        //                    GVK_PostEventDetails._102Integration dt = new GVK_PostEventDetails._102Integration();
        //                    long MotherID = (long)row["beneficiary_id"];
        //                    int PregnancyID = Convert.ToInt32(row["pregnancyid"].ToString());
        //                    string ChildID = row["childid"].ToString();
        //                    int EventTypeID = 0;
        //                    try
        //                    {
        //                        EventTypeID = Convert.ToInt32(row["EventTypeId"].ToString());
        //                    }
        //                    catch (Exception ex)
        //                    {

        //                    }
        //                    int EventID = 0;
        //                    try
        //                    {
        //                        EventID = Convert.ToInt32(row["EventSubTypeId"].ToString());
        //                    }
        //                    catch (Exception ex)
        //                    {

        //                    }
        //                    string PickupTime = row["pickup_time"].ToString();
        //                    string DropTime = row["drop_reach_time"].ToString();
        //                    int DistrictID = 0;
        //                    try { DistrictID = Convert.ToInt32(row["DistrictID"].ToString()); } catch (Exception ex) { }
        //                    int HCID = 0; try { HCID = Convert.ToInt32(row["dest_location_hosp_id"].ToString()); } catch (Exception ex) { }
        //                    string AdhaarFlag = row["IsAdharverified"].ToString();
        //                    string Vehicleno = row["vehicle_no"].ToString();
        //                    int Calls = 1;
        //                    string LogUserId = "EmriUSer1";
        //                    TraceService("Posted Values to PostEventDetails are ::" + "KCRKIT" + " ~ " + "102@KCRKIT" + " ~ " + MotherID + " ~ " + PregnancyID + " ~ " + ChildID + " ~ " + EventTypeID + " ~ " + EventID + " ~ " + PickupTime + " ~ " + DropTime + " ~ " + DistrictID + " ~ " + HCID + " ~ " + AdhaarFlag + " ~ " + Vehicleno + " ~ " + Calls + " ~ " + LogUserId);
        //                    try
        //                    {
        //                        bool b = dt.PostEventDetails("KCRKIT", "102@KCRKIT", MotherID, PregnancyID, ChildID, EventTypeID, EventID, PickupTime, DropTime, DistrictID, HCID, AdhaarFlag, Vehicleno, Calls, LogUserId);
        //                        executeInsertStatement("insert into  t_posteventDetails(MotherID, status,PregnancyID ) values ('" + MotherID + "', '" + b.ToString() + "' , '" + PregnancyID + "');");
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        TraceService(ex.ToString() + " Exception Raised for values ::" + "KCRKIT" + " ~ " + "102@KCRKIT" + " ~ " + MotherID + " ~ " + PregnancyID + " ~ " + ChildID + " ~ " + EventTypeID + " ~ " + EventID + " ~ " + PickupTime + " ~ " + DropTime + " ~ " + DistrictID + " ~ " + HCID + " ~ " + AdhaarFlag + " ~ " + Vehicleno + " ~ " + Calls + " ~ " + LogUserId);
        //                    }
        //                }

        //                pushPostEventDetails(dtWsSyncDetails);
        //            }
        //            else
        //            {
        //                string updatestmt = "update wssyncstatus set date = now(),status ='Stopped', lastcheckdate = now(),currentStatus = 'Processed', remarks= 'Processed data for today' " + DateTime.Now.ToString("yyyy-MM-dd") + " where isactive=1;";
        //                executeInsertStatement(updatestmt);

        //                TraceService("Rows count is zero or less than zero");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            TraceService("Exception Raised " + ex.ToString());
        //        }
        //    }
        //    else
        //    {
        //        string updatestmt = "update wssyncstatus set date = now(),status ='Stopped', lastcheckdate = now(),currentStatus = 'Processed', remarks= 'Processed data for today' " + DateTime.Now.ToString("yyyy-MM-dd") + " where isactive=1;";
        //        executeInsertStatement(updatestmt);

        //        TraceService("pushPostEventDetails is set to zero");
        //    }
        //}
        //private void InsertUpdatePregencyDetails(DataTable dta, int count)
        //{
        //    try
        //    {
        //        for (int i = 0; i < count - 1; i++)
        //        {
        //            try
        //            {

        //                MySqlConnection objMySqlConnection = null;
        //                objMySqlConnection = new MySqlConnection(connectionString);
        //                objMySqlConnection.Open();
        //                using (MySqlCommand cmd = objMySqlConnection.CreateCommand())
        //                {
        //                    DateTime deliverydate, DischargeDate, LMPDate, EDD, CreatedDate;
        //                    CreatedDate = deliverydate = DischargeDate = LMPDate = EDD = Convert.ToDateTime(defDate);
        //                    try { deliverydate = Convert.ToDateTime(dta.Rows[i]["DeliveryDate"].ToString()); } catch { }
        //                    try
        //                    {
        //                        DischargeDate = Convert.ToDateTime(dta.Rows[i]["DischargeDate"].ToString());
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    try
        //                    {
        //                        LMPDate = Convert.ToDateTime(dta.Rows[i]["LMPDate"].ToString());
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    try
        //                    {
        //                        EDD = Convert.ToDateTime(dta.Rows[i]["EDD"].ToString());
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    try
        //                    {
        //                        CreatedDate = Convert.ToDateTime(dta.Rows[count - 1]["CreatedDate"].ToString());
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    int DeliveryPalceType, DeliveryOutcomeID;
        //                    DeliveryPalceType = DeliveryOutcomeID = 0;
        //                    if (dta.Rows[i]["DeliveryPalceType"].ToString() != "")
        //                    {
        //                        DeliveryPalceType = Convert.ToInt32(dta.Rows[i]["DeliveryPalceType"].ToString());
        //                    }
        //                    if (dta.Rows[i]["DeliveryOutcomeID"].ToString() != "")
        //                    {
        //                        DeliveryOutcomeID = Convert.ToInt32(dta.Rows[i]["DeliveryOutcomeID"].ToString());
        //                    }
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.CommandText = "Insert_Update_PregencyDetails";
        //                    cmd.Parameters.AddWithValue("@Mother_ID", dta.Rows[i]["MotherID"].ToString());
        //                    cmd.Parameters.AddWithValue("@Pregnancy_ID", dta.Rows[i]["PregnancyID"].ToString());
        //                    cmd.Parameters.AddWithValue("@LMP_Date", LMPDate.ToString("yyyy-MM-dd HH:mm:ss"));
        //                    cmd.Parameters.AddWithValue("@EDD1", EDD.ToString("yyyy-MM-dd HH:mm:ss"));
        //                    cmd.Parameters.AddWithValue("@HighRisk_ID", dta.Rows[i]["HighRiskID"].ToString());
        //                    cmd.Parameters.AddWithValue("@Other_HighRisk", dta.Rows[i]["OtherHighRisk"].ToString());
        //                    cmd.Parameters.AddWithValue("@Delivery_Date", deliverydate.ToString("yyyy-MM-dd HH:mm:ss"));
        //                    cmd.Parameters.AddWithValue("@Delivery_PalceType", DeliveryPalceType);
        //                    cmd.Parameters.AddWithValue("@Delivery_OutcomeID", DeliveryOutcomeID);
        //                    cmd.Parameters.AddWithValue("@Discharge_Date", DischargeDate.ToString("yyyy-MM-dd HH:mm:ss"));
        //                    cmd.Parameters.AddWithValue("@Meternal_Outcome", dta.Rows[i]["MeternalOutcome"].ToString());
        //                    cmd.Parameters.AddWithValue("@ASHA_ID", dta.Rows[i]["ASHAID"].ToString());
        //                    cmd.Parameters.AddWithValue("@Created_By", dta.Rows[i]["CreatedBy"].ToString());
        //                    cmd.Parameters.AddWithValue("@Batch_Id", dta.Rows[count - 1]["BatchId"].ToString());
        //                    cmd.Parameters.AddWithValue("@Type_id", dta.Rows[count - 1]["Typeid"].ToString());
        //                    cmd.Parameters.AddWithValue("@Created_Date", CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
        //                    cmd.ExecuteNonQuery();
        //                }
        //                objMySqlConnection.Close();

        //            }
        //            catch (Exception ex)
        //            {
        //                TraceService_abnormal(ex.ToString());
        //            }

        //        }
        //        string batchselect = "insert into t_batchDetails(`batchid`,`Typeid`,`Count`,`Response`,`Createddate`,`Createdby`,`Updateddate`,`Updatedby`,`Isactive`) values ";
        //        batchselect = batchselect + "( '" + dta.Rows[count - 1]["batchid"].ToString() + "','" + dta.Rows[count - 1]["Typeid"].ToString() + "','" + dta.Rows[count - 1]["Count"].ToString() + "','" + dta.Rows[count - 1]["Response"].ToString() + "', ";
        //        batchselect = batchselect + " '" + dta.Rows[count - 1]["Createddate"].ToString() + "','" + dta.Rows[count - 1]["Createdby"].ToString() + "', ";
        //        batchselect = batchselect + "'" + dta.Rows[count - 1]["Updateddate"].ToString() + "','" + dta.Rows[count - 1]["Updatedby"].ToString() + "','" + dta.Rows[count - 1]["Isactive"].ToString() + "')";
        //        executeInsertStatement(batchselect);
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceService_abnormal(ex.ToString());
        //    }
        //}
        //private void InsertUpdateChildDetails(DataTable dta, int count)
        //{
        //    try
        //    {
        //        for (int i = 0; i < count - 1; i++)
        //        {
        //            try
        //            {
        //                DateTime DOB, DD, BCGDATE, Createddate, OPV0, HEPB, OPV1, PENTA1, OPV2, PENTA2, OPV3, PENTA3, IPV1, IPV2, JE, MEASLES, VITA;
        //                string DOBString, DeathDate, createdDate, BCGDATEStr, CreateddateStr;
        //                string OPV0str, HEPBstr, OPV1str, PENTA1str, OPV2str, PENTA2str, OPV3str, PENTA3str, IPV1str, IPV2str, JEstr, MEASLESstr, VITAstr;
        //                DOBString = DeathDate = createdDate = BCGDATEStr = CreateddateStr = OPV0str = HEPBstr = OPV1str = PENTA1str = OPV2str = PENTA2str = OPV3str = PENTA3str = IPV1str = IPV2str = JEstr = MEASLESstr = VITAstr = defDate;
        //                if (dta.Rows[i]["DateofBirth"].ToString() != "")
        //                {
        //                    string DateofBirthD = dta.Rows[i]["DateofBirth"].ToString().Replace("/", "-");
        //                    DOB = DateTime.Parse(DateofBirthD);
        //                    DOBString = DOB.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }

        //                if (dta.Rows[i]["DEATHDATE"].ToString() != "")
        //                {
        //                    string DEATHDATED = dta.Rows[i]["DEATHDATE"].ToString().Replace("/", "-");
        //                    DD = DateTime.Parse(DEATHDATED);
        //                    DeathDate = DD.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[i]["BCGDATE"].ToString() != "")
        //                {
        //                    string BCGDATED = dta.Rows[i]["BCGDATE"].ToString().Replace("/", "-");
        //                    BCGDATE = DateTime.Parse(BCGDATED);
        //                    BCGDATEStr = BCGDATE.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[count - 1]["Createddate"].ToString() != "")
        //                {
        //                    string CreateddateD = dta.Rows[count - 1]["Createddate"].ToString().Replace("/", "-");
        //                    Createddate = DateTime.Parse(CreateddateD);
        //                    CreateddateStr = Createddate.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[i]["OPV0"].ToString() != "")
        //                {
        //                    string OPV0D = dta.Rows[i]["OPV0"].ToString().Replace("/", "-");
        //                    OPV0 = DateTime.Parse(OPV0D);
        //                    OPV0str = OPV0.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[i]["HEPB"].ToString() != "")
        //                {
        //                    string tempdate = dta.Rows[i]["HEPB"].ToString().Replace("/", "-");
        //                    HEPB = DateTime.Parse(tempdate);
        //                    HEPBstr = HEPB.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[i]["OPV1"].ToString() != "")
        //                {
        //                    string OPV1D = dta.Rows[i]["OPV1"].ToString().Replace("/", "-");
        //                    OPV1 = DateTime.Parse(OPV1D);
        //                    OPV1str = OPV1.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[i]["PENTA1"].ToString() != "")
        //                {
        //                    string PENTA1D = dta.Rows[i]["PENTA1"].ToString().Replace("/", "-");
        //                    PENTA1 = DateTime.Parse(PENTA1D);
        //                    PENTA1str = PENTA1.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[i]["OPV2"].ToString() != "")
        //                {
        //                    string OPV2D = dta.Rows[i]["OPV2"].ToString().Replace("/", "-");
        //                    OPV2 = DateTime.Parse(OPV2D);
        //                    OPV2str = OPV2.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[i]["PENTA2"].ToString() != "")
        //                {
        //                    string PENTA2D = dta.Rows[i]["PENTA2"].ToString().Replace("/", "-");
        //                    PENTA2 = DateTime.Parse(PENTA2D);
        //                    PENTA2str = PENTA2.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[i]["OPV3"].ToString() != "")
        //                {
        //                    string OPV3D = dta.Rows[i]["OPV3"].ToString().Replace("/", "-");
        //                    OPV3 = DateTime.Parse(OPV3D);
        //                    OPV3str = OPV3.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[i]["PENTA3"].ToString() != "")
        //                {
        //                    string PENTA3D = dta.Rows[i]["PENTA3"].ToString().Replace("/", "-");
        //                    PENTA3 = DateTime.Parse(PENTA3D);
        //                    PENTA3str = PENTA3.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[i]["IPV1"].ToString() != "")
        //                {
        //                    string IPV1D = dta.Rows[i]["IPV1"].ToString().Replace("/", "-");
        //                    //"MM-dd-yyyy HH:mm:ss" 
        //                    IPV1 = DateTime.Parse(IPV1D);
        //                    IPV1str = IPV1.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[i]["IPV2"].ToString() != "")
        //                {
        //                    string IPV2D = dta.Rows[i]["IPV2"].ToString().Replace("/", "-");
        //                    IPV2 = DateTime.Parse(IPV2D);
        //                    IPV2str = IPV2.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[i]["JE"].ToString() != "")
        //                {
        //                    string JED = dta.Rows[i]["JE"].ToString().Replace("/", "-");
        //                    JE = DateTime.Parse(JED);
        //                    JEstr = JE.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[i]["MEASLES"].ToString() != "")
        //                {
        //                    string MEASLESD = dta.Rows[i]["MEASLES"].ToString().Replace("/", "-");
        //                    MEASLES = DateTime.Parse(MEASLESD);
        //                    MEASLESstr = MEASLES.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[i]["VITA"].ToString() != "")
        //                {
        //                    string VITAD = dta.Rows[i]["VITA"].ToString().Replace("/", "-");
        //                    VITA = DateTime.Parse(VITAD);
        //                    VITAstr = VITA.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                MySqlConnection objMySqlConnection = null;
        //                objMySqlConnection = new MySqlConnection(connectionString);
        //                objMySqlConnection.Open();
        //                using (MySqlCommand cmd = objMySqlConnection.CreateCommand())
        //                {
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.CommandText = "Insert_Update_Childdetails";
        //                    cmd.Parameters.AddWithValue("@MOTHER_ID", dta.Rows[i]["MOTHERID"].ToString());
        //                    cmd.Parameters.AddWithValue("@PREGNANCY_ID", dta.Rows[i]["PREGNANCYID"].ToString());
        //                    cmd.Parameters.AddWithValue("@Child_Id", dta.Rows[i]["ChildId"].ToString());
        //                    cmd.Parameters.AddWithValue("@CHILD_NAME", dta.Rows[i]["CHILDNAME"].ToString());
        //                    cmd.Parameters.AddWithValue("@GEN", dta.Rows[i]["GENDER"].ToString());
        //                    cmd.Parameters.AddWithValue("@Dateof_Birth", DOBString);
        //                    cmd.Parameters.AddWithValue("@IS_ALIVE", dta.Rows[i]["ISALIVE"].ToString());
        //                    cmd.Parameters.AddWithValue("@DEATH_DATE", DeathDate);
        //                    cmd.Parameters.AddWithValue("@DEATH_PLACE", dta.Rows[i]["DEATHPLACE"].ToString());
        //                    cmd.Parameters.AddWithValue("@BCG_DATE", BCGDATEStr);
        //                    cmd.Parameters.AddWithValue("@OPV_0", OPV0str);
        //                    cmd.Parameters.AddWithValue("@HEP_B", HEPBstr);
        //                    cmd.Parameters.AddWithValue("@OPV_1", OPV1str);
        //                    cmd.Parameters.AddWithValue("@PENTA_1", PENTA1str);
        //                    cmd.Parameters.AddWithValue("@OPV_2", OPV2str);
        //                    cmd.Parameters.AddWithValue("@PENTA_2", PENTA2str);
        //                    cmd.Parameters.AddWithValue("@OPV_3", OPV3str);
        //                    cmd.Parameters.AddWithValue("@PENTA_3", PENTA3str);
        //                    cmd.Parameters.AddWithValue("@IPV_1", IPV1str);
        //                    cmd.Parameters.AddWithValue("@IPV_2", IPV2str);
        //                    cmd.Parameters.AddWithValue("@JE1", JEstr);
        //                    cmd.Parameters.AddWithValue("@MEASLES1", MEASLESstr);
        //                    cmd.Parameters.AddWithValue("@VITA1", VITAstr);
        //                    cmd.Parameters.AddWithValue("@Batch_Id", dta.Rows[count - 1]["BatchId"].ToString());
        //                    cmd.Parameters.AddWithValue("@Type_id", dta.Rows[count - 1]["Typeid"].ToString());
        //                    cmd.Parameters.AddWithValue("@Created_date", CreateddateStr);
        //                    cmd.ExecuteNonQuery();
        //                }
        //                objMySqlConnection.Close();

        //            }
        //            catch (Exception ex)
        //            {
        //                TraceService_abnormal(ex.ToString());
        //            }

        //        }
        //        string batchselect = "insert into t_batchDetails(`batchid`,`Typeid`,`Count`,`Response`,`Createddate`,`Createdby`,`Updateddate`,`Updatedby`,`Isactive`) values ";
        //        batchselect = batchselect + "( '" + dta.Rows[count - 1]["batchid"].ToString() + "','" + dta.Rows[count - 1]["Typeid"].ToString() + "','" + dta.Rows[count - 1]["Count"].ToString() + "','" + dta.Rows[count - 1]["Response"].ToString() + "', ";
        //        batchselect = batchselect + " '" + dta.Rows[count - 1]["Createddate"].ToString() + "','" + dta.Rows[count - 1]["Createdby"].ToString() + "', ";
        //        batchselect = batchselect + "'" + dta.Rows[count - 1]["Updateddate"].ToString() + "','" + dta.Rows[count - 1]["Updatedby"].ToString() + "','" + dta.Rows[count - 1]["Isactive"].ToString() + "')";
        //        executeInsertStatement(batchselect);
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceService_abnormal(ex.ToString());
        //    }
        //}
        //private void InsertUpdateMotherData(DataTable dta, int count)
        //{
        //    try
        //    {
        //        for (int i = 0; i < count - 1; i++)
        //        {
        //            try
        //            {
        //                DateTime DD, CD;
        //                string DeathDate, createdDate;
        //                DeathDate = createdDate = defDate;

        //                if (dta.Rows[i]["DeathDate"].ToString() != "")
        //                {
        //                    DD = DateTime.Parse(dta.Rows[i]["DeathDate"].ToString());
        //                    DeathDate = DD.ToString("yyyy-MM-dd  HH:mm:ss");
        //                }
        //                if (dta.Rows[count - 1]["Createddate"].ToString() != "")
        //                {
        //                    CD = DateTime.Parse(dta.Rows[count - 1]["Createddate"].ToString());
        //                    createdDate = CD.ToString("yyyy-MM-dd  HH:mm:ss");

        //                }
        //                MySqlConnection objMySqlConnection = null;
        //                objMySqlConnection = new MySqlConnection(connectionString);
        //                objMySqlConnection.Open();
        //                using (MySqlCommand cmd = objMySqlConnection.CreateCommand())
        //                {
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.CommandText = "Insert_Update_MotherDetails";
        //                    cmd.Parameters.AddWithValue("@Mother_ID", dta.Rows[i]["MotherID"].ToString());
        //                    cmd.Parameters.AddWithValue("@Mother_Name", dta.Rows[i]["MotherName"].ToString());
        //                    cmd.Parameters.AddWithValue("@Husband_Name", dta.Rows[i]["HusbandName"].ToString());
        //                    cmd.Parameters.AddWithValue("@District_ID", dta.Rows[i]["DistrictID"].ToString());
        //                    cmd.Parameters.AddWithValue("@Mandal_ID", dta.Rows[i]["MandalID"].ToString());
        //                    cmd.Parameters.AddWithValue("@Village_ID", dta.Rows[i]["VillageID"].ToString());
        //                    cmd.Parameters.AddWithValue("@Address1", dta.Rows[i]["Address"].ToString());
        //                    cmd.Parameters.AddWithValue("@Age1", dta.Rows[i]["Age"].ToString());
        //                    cmd.Parameters.AddWithValue("@Registration_Date", dta.Rows[i]["RegistrationDate"].ToString());
        //                    cmd.Parameters.AddWithValue("@Death_Date", DeathDate);
        //                    cmd.Parameters.AddWithValue("@Death_Place", dta.Rows[i]["DeathPlace"].ToString());
        //                    cmd.Parameters.AddWithValue("@Death_Reason", dta.Rows[i]["DeathReason"].ToString());
        //                    cmd.Parameters.AddWithValue("@ANM_ID", dta.Rows[i]["ANMID"].ToString());
        //                    cmd.Parameters.AddWithValue("@OM_ID", dta.Rows[i]["NINID"].ToString());
        //                    cmd.Parameters.AddWithValue("@Mobile_No", dta.Rows[i]["Mobile"].ToString());
        //                    cmd.Parameters.AddWithValue("@HCDistId", dta.Rows[i]["HCDistId"].ToString());
        //                    cmd.Parameters.AddWithValue("@HCID", dta.Rows[i]["HCID"].ToString());
        //                    cmd.Parameters.AddWithValue("@Batch_Id", dta.Rows[count - 1]["BatchId"].ToString());
        //                    cmd.Parameters.AddWithValue("@Type_Id", dta.Rows[count - 1]["Typeid"].ToString());
        //                    cmd.Parameters.AddWithValue("@created_time", createdDate);
        //                    cmd.ExecuteNonQuery();
        //                }
        //                objMySqlConnection.Close();

        //            }
        //            catch (Exception ex)
        //            {
        //                TraceService_abnormal(ex.ToString());
        //            }

        //        }
        //        string batchselect = "insert into t_batchDetails(`batchid`,`Typeid`,`Count`,`Response`,`Createddate`,`Createdby`,`Updateddate`,`Updatedby`,`Isactive`) values ";
        //        batchselect = batchselect + "( '" + dta.Rows[count - 1]["batchid"].ToString() + "','" + dta.Rows[count - 1]["Typeid"].ToString() + "','" + dta.Rows[count - 1]["Count"].ToString() + "','" + dta.Rows[count - 1]["Response"].ToString() + "', ";
        //        batchselect = batchselect + " '" + dta.Rows[count - 1]["Createddate"].ToString() + "','" + dta.Rows[count - 1]["Createdby"].ToString() + "', ";
        //        batchselect = batchselect + "'" + dta.Rows[count - 1]["Updateddate"].ToString() + "','" + dta.Rows[count - 1]["Updatedby"].ToString() + "','" + dta.Rows[count - 1]["Isactive"].ToString() + "')";
        //        executeInsertStatement(batchselect);
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceService_abnormal(ex.ToString());
        //    }
        //}
        //public int executeInsertStatement(string insertStmt)
        //{
        //    try
        //    {
        //        int i = 0;
        //        using (MySqlConnection conn = new MySqlConnection(connectionString))
        //        {
        //            using (MySqlCommand comm = new MySqlCommand())
        //            {
        //                comm.Connection = conn;
        //                comm.CommandText = insertStmt;
        //                try
        //                {
        //                    conn.Open();
        //                    i = comm.ExecuteNonQuery();
        //                    TraceService(insertStmt);
        //                    return i;
        //                }
        //                catch (MySqlException ex)
        //                {
        //                    TraceService_abnormal(" executeInsertStatement " + ex.ToString() + insertStmt);
        //                    return i;
        //                }
        //                finally
        //                {
        //                    conn.Close();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceService_abnormal(" executeInsertStatement " + ex.ToString());
        //        return 0;
        //    }
        //}
        //private DataTable executeSelectStmt(string selectStmt)
        //{

        //    DataTable dtSyncData = new DataTable();
        //    MySqlConnection connection = null;
        //    try
        //    {
        //        connection = new MySqlConnection(connectionString);
        //        connection.Open();
        //        MySqlDataAdapter dataAdapter = new MySqlDataAdapter();
        //        dataAdapter.SelectCommand = new MySqlCommand(selectStmt, connection);
        //        dataAdapter.Fill(dtSyncData);
        //        return dtSyncData;
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceService(" executeSelectStmt() " + ex.ToString() + selectStmt);
        //        return null;
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }
        //}
        protected override void OnStop()
        {
            timer.Enabled = false;
            helper.traceService("stopping service");
        }
        //public void traceService(string content)
        //{
        //    try
        //    {
        //        FileStream fs = new FileStream(@"F:\KcrKit102Log\logException.txt", FileMode.OpenOrCreate, FileAccess.Write);
        //        //set up a streamwriter for adding text
        //        StreamWriter sw = new StreamWriter(fs);
        //        //find the end of the underlying filestream
        //        sw.BaseStream.Seek(0, SeekOrigin.End);
        //        //add the text
        //        sw.WriteLine(content);
        //        //add the text to the underlying filestream
        //        sw.Flush();
        //        //close the writer
        //        sw.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceService_abnormal(" exception in traceService " + ex.ToString());
        //    }

        //}
        //private void TraceService(string content)
        //{
        //    string str = @"F:\KcrKit102Log\" + DateTime.Today.ToString("yyyy-MM-dd") + @"\Log.txt";
        //    string path1 = str.Substring(0, str.LastIndexOf("\\"));
        //    string path2 = str.Substring(0, str.LastIndexOf(".txt")) + "-" + DateTime.Today.ToString("yyyy-MM-dd") + ".txt";
        //    try
        //    {
        //        if (!Directory.Exists(path1))
        //            Directory.CreateDirectory(path1);
        //        if (path2.Length >= Convert.ToInt32(4000000))
        //        {
        //            path2 = str.Substring(0, str.LastIndexOf(".txt")) + "-" + "2" + ".txt";
        //        }
        //        StreamWriter streamWriter = File.AppendText(path2);
        //        streamWriter.WriteLine("====================" + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString());
        //        streamWriter.WriteLine(content.ToString());
        //        streamWriter.WriteLine("=======================================================================");
        //        streamWriter.WriteLine();
        //        streamWriter.WriteLine();
        //        streamWriter.WriteLine();
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //    }

        //    catch (Exception ex)
        //    {
        //        traceService(ex.ToString());
        //    }
        //}
        //private void TraceService_result(string content)
        //{
        //    string str = @"F:\KcrKit102Log\" + DateTime.Today.ToString("yyyy-MM-dd") + @"\Log_ReceivedResult.txt";
        //    string path1 = str.Substring(0, str.LastIndexOf("\\"));
        //    string path2 = str.Substring(0, str.LastIndexOf(".txt")) + "-" + DateTime.Today.ToString("yyyy-MM-dd") + ".txt";
        //    try
        //    {
        //        if (!Directory.Exists(path1))
        //            Directory.CreateDirectory(path1);
        //        if (path2.Length >= Convert.ToInt32(4000000))
        //        {
        //            path2 = str.Substring(0, str.LastIndexOf(".txt")) + "-" + "2" + ".txt";
        //        }
        //        StreamWriter streamWriter = File.AppendText(path2);
        //        streamWriter.WriteLine("====================" + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString());
        //        streamWriter.WriteLine(content.ToString());
        //        streamWriter.WriteLine("=======================================================================");
        //        streamWriter.WriteLine();
        //        streamWriter.WriteLine();
        //        streamWriter.WriteLine();
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //    }

        //    catch (Exception ex)
        //    {
        //        traceService(ex.ToString());
        //    }
        //}
        //private void TraceService_abnormal(string content)
        //{
        //    string str = @"F:\KcrKit102Log\" + DateTime.Today.ToString("yyyy-MM-dd") + @"\Log_abnormal.txt";
        //    string path1 = str.Substring(0, str.LastIndexOf("\\"));
        //    string path2 = str.Substring(0, str.LastIndexOf(".txt")) + "-" + DateTime.Today.ToString("yyyy-MM-dd") + ".txt";
        //    try
        //    {
        //        if (!Directory.Exists(path1))
        //            Directory.CreateDirectory(path1);
        //        if (path2.Length >= Convert.ToInt32(4000000))
        //        {
        //            path2 = str.Substring(0, str.LastIndexOf(".txt")) + "-" + "2" + ".txt";
        //        }
        //        StreamWriter streamWriter = File.AppendText(path2);
        //        streamWriter.WriteLine("====================" + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString());
        //        streamWriter.WriteLine(content.ToString());
        //        streamWriter.WriteLine("=======================================================================");
        //        streamWriter.WriteLine();
        //        streamWriter.WriteLine();
        //        streamWriter.WriteLine();
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //    }

        //    catch (Exception ex)
        //    {
        //        traceService(ex.ToString());
        //    }
        //}
    }
}
