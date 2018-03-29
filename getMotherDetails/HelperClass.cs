using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;

namespace getMotherDetails
{
    public class HelperClass
    {
        public GetChildClassDetails ChildDetails { get; } = new GetChildClassDetails();
        private readonly string connectionString = "server=10.11.0.16;userid=emri;password=emri;database=emri;Convert Zero Datetime=True";
        public string Dateformat { get; } = "MM/dd/yyyy HH:mm:ss";
        private const string DefDate = "1900-01-01 00:00";

        public void traceService(string content)
        {
            try
            {
                var fs = new FileStream(@"F:\KcrKit102Log\logException.txt", FileMode.OpenOrCreate, FileAccess.Write);
                WriteToFile(content, fs);
            }
            catch (Exception ex)
            {
                TraceService_abnormal(" exception in traceService " + ex);
            }

        }

        private static void WriteToFile(string content, FileStream fs)
        {
            //set up a streamwriter for adding text
            using (var sw = new StreamWriter(fs))
            {
                sw.BaseStream.Seek(0, SeekOrigin.End);
                //add the text
                sw.WriteLine(content);
                //add the text to the underlying filestream
                sw.Flush();
                //close the writer
                sw.Close();
            }
        }

        public void TraceService_abnormal(string content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            string str = @"F:\KcrKit102Log\" + DateTime.Today.ToString("yyyy-MM-dd") + @"\Log_abnormal.txt";
            string path1 = str.Substring(0, str.LastIndexOf("\\", StringComparison.Ordinal));
            string path2 = str.Substring(0, str.LastIndexOf(".txt", StringComparison.Ordinal)) + "-" + DateTime.Today.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                if (!Directory.Exists(path1))
                    Directory.CreateDirectory(path1);
                if (path2.Length >= 4000000)
                {
                    path2 = str.Substring(0, str.LastIndexOf(".txt", StringComparison.Ordinal)) + "-" + "2" + ".txt";
                }

                using (var streamWriter = File.AppendText(path2))
                {
                    streamWriter.WriteLine("====================" + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString());
                    streamWriter.WriteLine(content);
                    streamWriter.WriteLine("=======================================================================");
                    streamWriter.WriteLine();
                    streamWriter.WriteLine();
                    streamWriter.WriteLine();
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            catch (Exception ex)
            {
                traceService(ex.ToString());
            }
        }

        public void TraceService(string content)
        {
            string str = @"F:\KcrKit102Log\" + DateTime.Today.ToString("yyyy-MM-dd") + @"\Log.txt";
            string path1 = str.Substring(0, str.LastIndexOf("\\", StringComparison.Ordinal));
            string path2 = str.Substring(0, str.LastIndexOf(".txt", StringComparison.Ordinal)) + "-" + DateTime.Today.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                if (!Directory.Exists(path1))
                    Directory.CreateDirectory(path1);
                if (path2.Length >= Convert.ToInt32(4000000))
                {
                    path2 = str.Substring(0, str.LastIndexOf(".txt", StringComparison.Ordinal)) + "-" + "2" + ".txt";
                }
                var streamWriter = File.AppendText(path2);
                streamWriter.WriteLine("====================" + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString());
                streamWriter.WriteLine(content.ToString());
                streamWriter.WriteLine("=======================================================================");
                streamWriter.WriteLine();
                streamWriter.WriteLine();
                streamWriter.WriteLine();
                streamWriter.Flush();
                streamWriter.Close();
            }

            catch (Exception ex)
            {
                traceService(ex.ToString());
            }
        }
        public void TraceService_result(string content)
        {
            string str = @"F:\KcrKit102Log\" + DateTime.Today.ToString("yyyy-MM-dd") + @"\Log_ReceivedResult.txt";
            string path1 = str.Substring(0, str.LastIndexOf("\\", StringComparison.Ordinal));
            string path2 = str.Substring(0, str.LastIndexOf(".txt", StringComparison.Ordinal)) + "-" + DateTime.Today.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                if (!Directory.Exists(path1))
                    Directory.CreateDirectory(path1);
                if (path2.Length >= 4000000)
                {
                    path2 = str.Substring(0, str.LastIndexOf(".txt", StringComparison.Ordinal)) + "-" + "2" + ".txt";
                }
                var streamWriter = File.AppendText(path2);
                streamWriter.WriteLine("====================" + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString());
                streamWriter.WriteLine(content);
                streamWriter.WriteLine("=======================================================================");
                streamWriter.WriteLine();
                streamWriter.WriteLine();
                streamWriter.WriteLine();
                streamWriter.Flush();
                streamWriter.Close();
            }

            catch (Exception ex)
            {
                traceService(ex.ToString());
            }
        }

        public int ExecuteInsertStatement(string insertStmt)
        {
            try
            {

                int i = 0;
                using (var conn = new MySqlConnection(connectionString))
                {
                    using (var comm = new MySqlCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = insertStmt;
                        try
                        {
                            conn.Open();
                            i = comm.ExecuteNonQuery();
                            TraceService(insertStmt);
                            return i;
                        }
                        catch (MySqlException ex)
                        {
                            TraceService_abnormal(" executeInsertStatement " + ex + insertStmt);
                            return i;
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TraceService_abnormal(" executeInsertStatement " + ex);
                return 0;
            }
        }

        public void InsertUpdatePregencyDetails(DataTable dta, int count)
        {
            try
            {
                for (int i = 0; i < count - 1; i++)
                {
                    try
                    {
                        var objMySqlConnection = new MySqlConnection(connectionString);
                        objMySqlConnection.Open();
                        using (var cmd = objMySqlConnection.CreateCommand())
                        {
                            DateTime deliverydate, dischargeDate, lmpDate, edd;
                            var createdDate = deliverydate = dischargeDate = lmpDate = edd = Convert.ToDateTime(DefDate);
                            try
                            {
                                deliverydate = Convert.ToDateTime(dta.Rows[i]["DeliveryDate"].ToString());
                                dischargeDate = Convert.ToDateTime(dta.Rows[i]["DischargeDate"].ToString());
                                lmpDate = Convert.ToDateTime(dta.Rows[i]["LMPDate"].ToString());
                                edd = Convert.ToDateTime(dta.Rows[i]["EDD"].ToString());
                                createdDate = Convert.ToDateTime(dta.Rows[count - 1]["CreatedDate"].ToString());
                            }
                            catch
                            {
                                // ignored
                            }
                            int deliveryOutcomeId;
                            var deliveryPalceType = deliveryOutcomeId = 0;
                            if (dta.Rows[i]["DeliveryPalceType"].ToString() != "")
                            {
                                deliveryPalceType = Convert.ToInt32(dta.Rows[i]["DeliveryPalceType"].ToString());
                            }
                            if (dta.Rows[i]["DeliveryOutcomeID"].ToString() != "")
                            {
                                deliveryOutcomeId = Convert.ToInt32(dta.Rows[i]["DeliveryOutcomeID"].ToString());
                            }
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "Insert_Update_PregencyDetails";
                            cmd.Parameters.AddWithValue("@Mother_ID", dta.Rows[i]["MotherID"].ToString());
                            cmd.Parameters.AddWithValue("@Pregnancy_ID", dta.Rows[i]["PregnancyID"].ToString());
                            cmd.Parameters.AddWithValue("@LMP_Date", lmpDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@EDD1", edd.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@HighRisk_ID", dta.Rows[i]["HighRiskID"].ToString());
                            cmd.Parameters.AddWithValue("@Other_HighRisk", dta.Rows[i]["OtherHighRisk"].ToString());
                            cmd.Parameters.AddWithValue("@Delivery_Date", deliverydate.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@Delivery_PalceType", deliveryPalceType);
                            cmd.Parameters.AddWithValue("@Delivery_OutcomeID", deliveryOutcomeId);
                            cmd.Parameters.AddWithValue("@Discharge_Date", dischargeDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@Meternal_Outcome", dta.Rows[i]["MeternalOutcome"].ToString());
                            cmd.Parameters.AddWithValue("@ASHA_ID", dta.Rows[i]["ASHAID"].ToString());
                            cmd.Parameters.AddWithValue("@Created_By", dta.Rows[i]["CreatedBy"].ToString());
                            cmd.Parameters.AddWithValue("@Batch_Id", dta.Rows[count - 1]["BatchId"].ToString());
                            cmd.Parameters.AddWithValue("@Type_id", dta.Rows[count - 1]["Typeid"].ToString());
                            cmd.Parameters.AddWithValue("@Created_Date", createdDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.ExecuteNonQuery();
                        }
                        objMySqlConnection.Close();

                    }
                    catch (Exception ex)
                    {
                        TraceService_abnormal(ex.ToString());
                    }

                }
                var batchselect = "insert into t_batchDetails(`batchid`,`Typeid`,`Count`,`Response`,`Createddate`,`Createdby`,`Updateddate`,`Updatedby`,`Isactive`) values ";
                batchselect = batchselect + "( '" + dta.Rows[count - 1]["batchid"].ToString() + "','" + dta.Rows[count - 1]["Typeid"].ToString() + "','" + dta.Rows[count - 1]["Count"].ToString() + "','" + dta.Rows[count - 1]["Response"].ToString() + "', ";
                batchselect = batchselect + " '" + dta.Rows[count - 1]["Createddate"].ToString() + "','" + dta.Rows[count - 1]["Createdby"].ToString() + "', ";
                batchselect = batchselect + "'" + dta.Rows[count - 1]["Updateddate"].ToString() + "','" + dta.Rows[count - 1]["Updatedby"].ToString() + "','" + dta.Rows[count - 1]["Isactive"].ToString() + "')";
                ExecuteInsertStatement(batchselect);
            }
            catch (Exception ex)
            {
                TraceService_abnormal(ex.ToString());
            }
        }

        public void InsertUpdateAncDetails(DataTable dta, int count)
        {
            try
            {
                for (int i = 0; i < count - 1; i++)
                {
                    try
                    {
                        DateTime checkupDate;
                        var createdDate = checkupDate = Convert.ToDateTime(DefDate);
                        try { checkupDate = Convert.ToDateTime(dta.Rows[i]["CheckupDate"].ToString()); }
                        catch
                        {
                            // ignored
                        }

                        try { createdDate = Convert.ToDateTime(dta.Rows[i]["CreatedDate"].ToString()); }
                        catch
                        {
                            // ignored
                        }

                        MySqlConnection objMySqlConnection = null;
                        objMySqlConnection = new MySqlConnection(connectionString);
                        objMySqlConnection.Open();
                        using (var cmd = objMySqlConnection.CreateCommand())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "Insert_Update_ANCDetails";
                            cmd.Parameters.AddWithValue("@Mother_ID", dta.Rows[i]["MotherID"].ToString());
                            cmd.Parameters.AddWithValue("@Pregnancy_ID", dta.Rows[i]["PregnancyID"].ToString());
                            cmd.Parameters.AddWithValue("@ANC_ID", dta.Rows[i]["ANCID"].ToString());
                            cmd.Parameters.AddWithValue("@Checkup_Date", checkupDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@Place_Type", dta.Rows[i]["PlaceType"].ToString());
                            cmd.Parameters.AddWithValue("@Checkup_Place", dta.Rows[i]["CheckupPlace"].ToString());
                            cmd.Parameters.AddWithValue("@Place_Name", dta.Rows[i]["PlaceName"].ToString());
                            cmd.Parameters.AddWithValue("@ANC_by", dta.Rows[i]["ANCby"].ToString());
                            cmd.Parameters.AddWithValue("@Batch_Id", dta.Rows[count - 1]["BatchId"].ToString());
                            cmd.Parameters.AddWithValue("@Type_id", dta.Rows[count - 1]["Typeid"].ToString());
                            cmd.Parameters.AddWithValue("@Created_Date", createdDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.ExecuteNonQuery();
                        }
                        objMySqlConnection.Close();

                    }
                    catch (Exception ex)
                    {
                        TraceService_abnormal(ex.ToString());
                    }

                }
                string batchselect = "insert into t_batchDetails(`batchid`,`Typeid`,`Count`,`Response`,`Createddate`,`Createdby`,`Updateddate`,`Updatedby`,`Isactive`) values ";
                batchselect = batchselect + "( '" + dta.Rows[count - 1]["batchid"] + "','" + dta.Rows[count - 1]["Typeid"].ToString() + "','" + dta.Rows[count - 1]["Count"].ToString() + "','" + dta.Rows[count - 1]["Response"].ToString() + "', ";
                batchselect = batchselect + " '" + dta.Rows[count - 1]["Createddate"] + "','" + dta.Rows[count - 1]["Createdby"].ToString() + "', ";
                batchselect = batchselect + "'" + dta.Rows[count - 1]["Updateddate"] + "','" + dta.Rows[count - 1]["Updatedby"].ToString() + "','" + dta.Rows[count - 1]["Isactive"].ToString() + "')";
                ExecuteInsertStatement(batchselect);
            }
            catch (Exception ex)
            {
                TraceService_abnormal(ex.ToString());
            }
        }
        public void PushPostEventDetails(DataTable dtWsSyncDetails)
        {
            int eventTypeId = 0;
            int eventId = 0;
            int districtId = 0;
            int hcid = 0;
            if (dtWsSyncDetails.Rows[0]["pushPostEventDetails"].ToString() == "1")
            {

                try
                {
                    var cmd = " SELECT bcd.beneficiary_id beneficiary_id, pd.pregnancyid pregnancyid, IFNULL(cdr.childid, '') childid, ";
                    cmd = cmd + " bcd.`EventTypeId` EventTypeId,bcd.`EventSubTypeId` EventSubTypeId, DATE_FORMAT(bcd.pickup_time, '%m/%d/%Y %H:%i')  pickup_time, ";
                    cmd = cmd + "DATE_FORMAT(bcd.drop_reach_time, '%m/%d/%Y %H:%i')  drop_reach_time, bcd.DistrictID DistrictID, bcd.dest_location_hosp_id dest_location_hosp_id, bcd.IsAdharverified IsAdharverified, ";
                    cmd = cmd + " va.vehicle_no vehicle_no FROM beneficiary_case_details bcd JOIN pregnancydetails pd ON pd.`MotherID` = bcd.beneficiary_id ";
                    cmd = cmd + " JOIN vehicle_assignment va ON va.`TripId` = bcd.`TripId` ";
                    cmd = cmd + " LEFT JOIN childregistration cdr ON cdr.`MotherID` = bcd.`beneficiary_id` LEFT JOIN t_posteventDetails pe ON pe.`MotherID` = bcd.beneficiary_id WHERE pe.MotherID IS NULL OR pe.status = 'False' limit 0,100";
                    var dtData = ExecuteSelectStmt(cmd);


                    if (dtData.Rows.Count > 0)
                    {
                        var updatestmt = "update wssyncstatus set status ='Processing', lastcheckdate = now(),currentStatus = 'pushPostEventDetails' where isactive=1;";
                        ExecuteInsertStatement(updatestmt);


                        foreach (DataRow row in dtData.Rows)
                        {
                            var dt = new GVK_PostEventDetails._102Integration();
                            long motherId = (long)row["beneficiary_id"];
                            int pregnancyId = Convert.ToInt32(row["pregnancyid"].ToString());
                            string childId = row["childid"].ToString();
                            string pickupTime = row["pickup_time"].ToString();
                            string dropTime = row["drop_reach_time"].ToString();
                            string adhaarFlag = row["IsAdharverified"].ToString();
                            string vehicleno = row["vehicle_no"].ToString();
                            int calls = 1;
                            string logUserId = "EmriUSer1";
                            try
                            {
                                eventTypeId = Convert.ToInt32(row["EventTypeId"].ToString());
                                eventId = Convert.ToInt32(row["EventSubTypeId"].ToString());
                                districtId = Convert.ToInt32(row["DistrictID"].ToString());
                                hcid = Convert.ToInt32(row["dest_location_hosp_id"].ToString());
                            }
                            catch (Exception)
                            {
                                // ignored
                            }


                            TraceService("Posted Values to PostEventDetails are ::" + "KCRKIT" + " ~ " + "102@KCRKIT" + " ~ " + motherId + " ~ " + pregnancyId + " ~ " + childId + " ~ " + eventTypeId + " ~ " + eventId + " ~ " + pickupTime + " ~ " + dropTime + " ~ " + districtId + " ~ " + hcid + " ~ " + adhaarFlag + " ~ " + vehicleno + " ~ " + calls + " ~ " + logUserId);
                            try
                            {
                                bool b = dt.PostEventDetails("KCRKIT", "102@KCRKIT", motherId, pregnancyId, childId, eventTypeId, eventId, pickupTime, dropTime, districtId, hcid, adhaarFlag, vehicleno, calls, logUserId);
                                ExecuteInsertStatement("insert into  t_posteventDetails(MotherID, status,PregnancyID ) values ('" + motherId + "', '" + b + "' , '" + pregnancyId + "');");
                            }
                            catch (Exception ex)
                            {
                                TraceService(ex + " Exception Raised for values ::" + "KCRKIT" + " ~ " + "102@KCRKIT" + " ~ " + motherId + " ~ " + pregnancyId + " ~ " + childId + " ~ " + eventTypeId + " ~ " + eventId + " ~ " + pickupTime + " ~ " + dropTime + " ~ " + districtId + " ~ " + hcid + " ~ " + adhaarFlag + " ~ " + vehicleno + " ~ " + calls + " ~ " + logUserId);
                            }
                        }

                        PushPostEventDetails(dtWsSyncDetails);
                    }
                    else
                    {
                        string updatestmt = "update wssyncstatus set date = now(),status ='Stopped', lastcheckdate = now(),currentStatus = 'Processed', remarks= 'Processed data for today' " + DateTime.Now.ToString("yyyy-MM-dd") + " where isactive=1;";
                        ExecuteInsertStatement(updatestmt);

                        TraceService("Rows count is zero or less than zero");
                    }
                }
                catch (Exception ex)
                {
                    TraceService("Exception Raised " + ex);
                }
            }
            else
            {
                string updatestmt = "update wssyncstatus set date = now(),status ='Stopped', lastcheckdate = now(),currentStatus = 'Processed', remarks= 'Processed data for today' " + DateTime.Now.ToString("yyyy-MM-dd") + " where isactive=1;";
                ExecuteInsertStatement(updatestmt);

                TraceService("pushPostEventDetails is set to zero");
            }
        }

        public DataTable ExecuteSelectStmt(string selectStmt)
        {

            DataTable dtSyncData = new DataTable();
            using (var connection = new MySqlConnection(connectionString))
            {
                var dataAdapter =
                    new MySqlDataAdapter { SelectCommand = new MySqlCommand(selectStmt, connection) };
                try
                {

                    connection.Open();
                    dataAdapter.Fill(dtSyncData);
                    return dtSyncData;
                }
                catch (Exception ex)
                {
                    TraceService(" executeSelectStmt() " + ex + selectStmt);
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void InsertUpdateChildDetails(DataTable dta, int count)
        {
            try
            {
                for (int i = 0; i < count - 1; i++)
                {
                    try
                    {
                        string deathDate, bcgdateStr, createddateStr;
                        string opv0Str, hepBstr, opv1Str, penta1Str, opv2Str, penta2Str, opv3Str, penta3Str, ipv1Str, ipv2Str, jEstr, measleSstr, vitAstr;
                        var dobString = deathDate = bcgdateStr = createddateStr = opv0Str = hepBstr = opv1Str = penta1Str = opv2Str = penta2Str = opv3Str = penta3Str = ipv1Str = ipv2Str = jEstr = measleSstr = vitAstr = DefDate;
                        if (dta.Rows[i]["DateofBirth"].ToString() != "")
                        {
                            string dateofBirthD = dta.Rows[i]["DateofBirth"].ToString().Replace("/", "-");
                            var dob = DateTime.Parse(dateofBirthD);
                            dobString = dob.ToString("yyyy-MM-dd  HH:mm:ss");
                        }

                        if (dta.Rows[i]["DEATHDATE"].ToString() != "")
                        {
                            string deathdated = dta.Rows[i]["DEATHDATE"].ToString().Replace("/", "-");
                            var dd = DateTime.Parse(deathdated);
                            deathDate = dd.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[i]["BCGDATE"].ToString() != "")
                        {
                            string bcgdated = dta.Rows[i]["BCGDATE"].ToString().Replace("/", "-");
                            var bcgdate = DateTime.Parse(bcgdated);
                            bcgdateStr = bcgdate.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[count - 1]["Createddate"].ToString() != "")
                        {
                            string createddateD = dta.Rows[count - 1]["Createddate"].ToString().Replace("/", "-");
                            var createddate = DateTime.Parse(createddateD);
                            createddateStr = createddate.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[i]["OPV0"].ToString() != "")
                        {
                            string opv0D = dta.Rows[i]["OPV0"].ToString().Replace("/", "-");
                            var opv0 = DateTime.Parse(opv0D);
                            opv0Str = opv0.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[i]["HEPB"].ToString() != "")
                        {
                            string tempdate = dta.Rows[i]["HEPB"].ToString().Replace("/", "-");
                            var hepb = DateTime.Parse(tempdate);
                            hepBstr = hepb.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[i]["OPV1"].ToString() != "")
                        {
                            string opv1D = dta.Rows[i]["OPV1"].ToString().Replace("/", "-");
                            var opv1 = DateTime.Parse(opv1D);
                            opv1Str = opv1.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[i]["PENTA1"].ToString() != "")
                        {
                            string penta1D = dta.Rows[i]["PENTA1"].ToString().Replace("/", "-");
                            var penta1 = DateTime.Parse(penta1D);
                            penta1Str = penta1.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[i]["OPV2"].ToString() != "")
                        {
                            string opv2D = dta.Rows[i]["OPV2"].ToString().Replace("/", "-");
                            var opv2 = DateTime.Parse(opv2D);
                            opv2Str = opv2.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[i]["PENTA2"].ToString() != "")
                        {
                            string penta2D = dta.Rows[i]["PENTA2"].ToString().Replace("/", "-");
                            var penta2 = DateTime.Parse(penta2D);
                            penta2Str = penta2.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[i]["OPV3"].ToString() != "")
                        {
                            string opv3D = dta.Rows[i]["OPV3"].ToString().Replace("/", "-");
                            var opv3 = DateTime.Parse(opv3D);
                            opv3Str = opv3.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[i]["PENTA3"].ToString() != "")
                        {
                            string penta3D = dta.Rows[i]["PENTA3"].ToString().Replace("/", "-");
                            var penta3 = DateTime.Parse(penta3D);
                            penta3Str = penta3.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[i]["IPV1"].ToString() != "")
                        {
                            string ipv1D = dta.Rows[i]["IPV1"].ToString().Replace("/", "-");
                            //"MM-dd-yyyy HH:mm:ss" 
                            var ipv1 = DateTime.Parse(ipv1D);
                            ipv1Str = ipv1.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[i]["IPV2"].ToString() != "")
                        {
                            string ipv2D = dta.Rows[i]["IPV2"].ToString().Replace("/", "-");
                            var ipv2 = DateTime.Parse(ipv2D);
                            ipv2Str = ipv2.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[i]["JE"].ToString() != "")
                        {
                            string jed = dta.Rows[i]["JE"].ToString().Replace("/", "-");
                            var je = DateTime.Parse(jed);
                            jEstr = je.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[i]["MEASLES"].ToString() != "")
                        {
                            string measlesd = dta.Rows[i]["MEASLES"].ToString().Replace("/", "-");
                            var measles = DateTime.Parse(measlesd);
                            measleSstr = measles.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[i]["VITA"].ToString() != "")
                        {
                            string vitad = dta.Rows[i]["VITA"].ToString().Replace("/", "-");
                            var vita = DateTime.Parse(vitad);
                            vitAstr = vita.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        using (var objMySqlConnection = new MySqlConnection(connectionString))
                        {
                            objMySqlConnection.Open();
                            using (var cmd = objMySqlConnection.CreateCommand())
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandText = "Insert_Update_Childdetails";
                                cmd.Parameters.AddWithValue("@MOTHER_ID", dta.Rows[i]["MOTHERID"].ToString());
                                cmd.Parameters.AddWithValue("@PREGNANCY_ID", dta.Rows[i]["PREGNANCYID"].ToString());
                                cmd.Parameters.AddWithValue("@Child_Id", dta.Rows[i]["ChildId"].ToString());
                                cmd.Parameters.AddWithValue("@CHILD_NAME", dta.Rows[i]["CHILDNAME"].ToString());
                                cmd.Parameters.AddWithValue("@GEN", dta.Rows[i]["GENDER"].ToString());
                                cmd.Parameters.AddWithValue("@Dateof_Birth", dobString);
                                cmd.Parameters.AddWithValue("@IS_ALIVE", dta.Rows[i]["ISALIVE"].ToString());
                                cmd.Parameters.AddWithValue("@DEATH_DATE", deathDate);
                                cmd.Parameters.AddWithValue("@DEATH_PLACE", dta.Rows[i]["DEATHPLACE"].ToString());
                                cmd.Parameters.AddWithValue("@BCG_DATE", bcgdateStr);
                                cmd.Parameters.AddWithValue("@OPV_0", opv0Str);
                                cmd.Parameters.AddWithValue("@HEP_B", hepBstr);
                                cmd.Parameters.AddWithValue("@OPV_1", opv1Str);
                                cmd.Parameters.AddWithValue("@PENTA_1", penta1Str);
                                cmd.Parameters.AddWithValue("@OPV_2", opv2Str);
                                cmd.Parameters.AddWithValue("@PENTA_2", penta2Str);
                                cmd.Parameters.AddWithValue("@OPV_3", opv3Str);
                                cmd.Parameters.AddWithValue("@PENTA_3", penta3Str);
                                cmd.Parameters.AddWithValue("@IPV_1", ipv1Str);
                                cmd.Parameters.AddWithValue("@IPV_2", ipv2Str);
                                cmd.Parameters.AddWithValue("@JE1", jEstr);
                                cmd.Parameters.AddWithValue("@MEASLES1", measleSstr);
                                cmd.Parameters.AddWithValue("@VITA1", vitAstr);
                                cmd.Parameters.AddWithValue("@Batch_Id", dta.Rows[count - 1]["BatchId"].ToString());
                                cmd.Parameters.AddWithValue("@Type_id", dta.Rows[count - 1]["Typeid"].ToString());
                                cmd.Parameters.AddWithValue("@Created_date", createddateStr);
                                cmd.ExecuteNonQuery();
                            }
                            objMySqlConnection.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        TraceService_abnormal(ex.ToString());
                    }

                }
                string batchselect = "insert into t_batchDetails(`batchid`,`Typeid`,`Count`,`Response`,`Createddate`,`Createdby`,`Updateddate`,`Updatedby`,`Isactive`) values ";
                batchselect = batchselect + "( '" + dta.Rows[count - 1]["batchid"] + "','" + dta.Rows[count - 1]["Typeid"].ToString() + "','" + dta.Rows[count - 1]["Count"].ToString() + "','" + dta.Rows[count - 1]["Response"].ToString() + "', ";
                batchselect = batchselect + " '" + dta.Rows[count - 1]["Createddate"] + "','" + dta.Rows[count - 1]["Createdby"].ToString() + "', ";
                batchselect = batchselect + "'" + dta.Rows[count - 1]["Updateddate"] + "','" + dta.Rows[count - 1]["Updatedby"].ToString() + "','" + dta.Rows[count - 1]["Isactive"].ToString() + "')";
                ExecuteInsertStatement(batchselect);
            }
            catch (Exception ex)
            {
                TraceService_abnormal(ex.ToString());
            }
        }
        public void InsertUpdateMotherData(DataTable dta, int count)
        {
            try
            {
                for (int i = 0; i < count - 1; i++)
                {
                    try
                    {
                        string createdDate;
                        var deathDate = createdDate = DefDate;

                        if (dta.Rows[i]["DeathDate"].ToString() != "")
                        {
                            var dd = DateTime.Parse(dta.Rows[i]["DeathDate"].ToString());
                            deathDate = dd.ToString("yyyy-MM-dd  HH:mm:ss");
                        }
                        if (dta.Rows[count - 1]["Createddate"].ToString() != "")
                        {
                            var cd = DateTime.Parse(dta.Rows[count - 1]["Createddate"].ToString());
                            createdDate = cd.ToString("yyyy-MM-dd  HH:mm:ss");
                        }

                        using (var objMySqlConnection = new MySqlConnection(connectionString))
                        {
                            objMySqlConnection.Open();
                            using (var cmd = objMySqlConnection.CreateCommand())
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandText = "Insert_Update_MotherDetails";
                                cmd.Parameters.AddWithValue("@Mother_ID", dta.Rows[i]["MotherID"].ToString());
                                cmd.Parameters.AddWithValue("@Mother_Name", dta.Rows[i]["MotherName"].ToString());
                                cmd.Parameters.AddWithValue("@Husband_Name", dta.Rows[i]["HusbandName"].ToString());
                                cmd.Parameters.AddWithValue("@District_ID", dta.Rows[i]["DistrictID"].ToString());
                                cmd.Parameters.AddWithValue("@Mandal_ID", dta.Rows[i]["MandalID"].ToString());
                                cmd.Parameters.AddWithValue("@Village_ID", dta.Rows[i]["VillageID"].ToString());
                                cmd.Parameters.AddWithValue("@Address1", dta.Rows[i]["Address"].ToString());
                                cmd.Parameters.AddWithValue("@Age1", dta.Rows[i]["Age"].ToString());
                                cmd.Parameters.AddWithValue("@Registration_Date", dta.Rows[i]["RegistrationDate"].ToString());
                                cmd.Parameters.AddWithValue("@Death_Date", deathDate);
                                cmd.Parameters.AddWithValue("@Death_Place", dta.Rows[i]["DeathPlace"].ToString());
                                cmd.Parameters.AddWithValue("@Death_Reason", dta.Rows[i]["DeathReason"].ToString());
                                cmd.Parameters.AddWithValue("@ANM_ID", dta.Rows[i]["ANMID"].ToString());
                                cmd.Parameters.AddWithValue("@OM_ID", dta.Rows[i]["NINID"].ToString());
                                cmd.Parameters.AddWithValue("@Mobile_No", dta.Rows[i]["Mobile"].ToString());
                                cmd.Parameters.AddWithValue("@HCDistId", dta.Rows[i]["HCDistId"].ToString());
                                cmd.Parameters.AddWithValue("@HCID", dta.Rows[i]["HCID"].ToString());
                                cmd.Parameters.AddWithValue("@Batch_Id", dta.Rows[count - 1]["BatchId"].ToString());
                                cmd.Parameters.AddWithValue("@Type_Id", dta.Rows[count - 1]["Typeid"].ToString());
                                cmd.Parameters.AddWithValue("@created_time", createdDate);
                                cmd.ExecuteNonQuery();
                            }
                            objMySqlConnection.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        TraceService_abnormal(ex.ToString());
                    }

                }
                string batchselect = "insert into t_batchDetails(`batchid`,`Typeid`,`Count`,`Response`,`Createddate`,`Createdby`,`Updateddate`,`Updatedby`,`Isactive`) values ";
                batchselect = batchselect + "( '" + dta.Rows[count - 1]["batchid"] + "','" + dta.Rows[count - 1]["Typeid"].ToString() + "','" + dta.Rows[count - 1]["Count"].ToString() + "','" + dta.Rows[count - 1]["Response"].ToString() + "', ";
                batchselect = batchselect + " '" + dta.Rows[count - 1]["Createddate"] + "','" + dta.Rows[count - 1]["Createdby"].ToString() + "', ";
                batchselect = batchselect + "'" + dta.Rows[count - 1]["Updateddate"] + "','" + dta.Rows[count - 1]["Updatedby"].ToString() + "','" + dta.Rows[count - 1]["Isactive"].ToString() + "')";
                ExecuteInsertStatement(batchselect);
            }
            catch (Exception ex)
            {
                TraceService_abnormal(ex.ToString());
            }
        }


    }
}
