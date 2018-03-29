using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace getMotherDetails
{
    public class PregnencyDetails
    {
        HelperClass helper = new HelperClass();
        ANCDetails ancDetails = new ANCDetails();
        public void GetPregancyDetails(DataTable dtWsSyncDetails)
        {
            if (dtWsSyncDetails.Rows[0]["GetPregancyDetails"].ToString() == "1")
            {
                try
                {
                    GVK_PregancyDetails._102Integration dt = new GVK_PregancyDetails._102Integration();
                    string data = dt.Get_PregancyDetails("KCRKIT", "102@KCRKIT");
                    helper.TraceService_result(data);
                    string updatestmt = "update wssyncstatus set status ='Processing', lastcheckdate = now(),currentStatus = 'GetPregancyDetails' where isactive=1;";
                    helper.executeInsertStatement(updatestmt);
                    DataTable dta = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));
                    if (dta == null || dta.Rows.Count <= 0)
                    {
                        helper.TraceService_abnormal("GetPregancyDetails -- Count is less than or equal to zero");
                        ancDetails.GetANCDetails(dtWsSyncDetails);
                    }
                    else
                    {
                        int count = dta.Rows.Count;
                        int count2 = Convert.ToInt32(dta.Rows[count - 1]["Count"].ToString());

                        if (count2 == 0)
                        {
                            helper.TraceService_abnormal("GetPregnancyDetails -- Count is or equal to zero");
                            ancDetails.GetANCDetails(dtWsSyncDetails);
                        }
                        if (count == count2 + 1)
                        {
                            helper.TraceService("GetPregancyDetails -- counts matched for batch id:" + dta.Rows[count - 1]["BatchId"].ToString());
                            helper.InsertUpdatePregencyDetails(dta, count);
                            GVK_UPDATE_SyncDetails._102Integration US = new GVK_UPDATE_SyncDetails._102Integration();
                            string res = US.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "1");
                            helper.TraceService("Response after updation for batch id :::" + dta.Rows[count - 1]["BatchId"].ToString() + ":::" + res);

                        }
                        else
                        {
                            helper.TraceService_abnormal("Counts are not matched for the batch with batch id " + dta.Rows[count - 1]["BatchId"].ToString());
                            GVK_UPDATE_SyncDetails._102Integration US = new GVK_UPDATE_SyncDetails._102Integration();
                            string res = US.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), System.DateTime.Now.ToString(), "0");
                            helper.TraceService("Response after updation for failed batch id :::" + dta.Rows[count - 1]["BatchId"].ToString() + ":::" + res);

                        }
                        GetPregancyDetails(dtWsSyncDetails);
                    }
                }
                catch (Exception ex)
                {
                    helper.TraceService_abnormal(ex.ToString());
                }
            }
            else
            {
                ancDetails.GetANCDetails(dtWsSyncDetails);
            }
        }
    }
}
