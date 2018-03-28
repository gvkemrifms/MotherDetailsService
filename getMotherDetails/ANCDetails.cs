using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace getMotherDetails
{
   public class ANCDetails: ServiceBase
    {
        HelperClass helper = new HelperClass();
        public void GetANCDetails(DataTable dtWsSyncDetails)
        {
            if (dtWsSyncDetails.Rows[0]["GetANCDetails"].ToString() == "1")
            {
                try
                {
                    GVK_ANCDetails._102Integration dt = new GVK_ANCDetails._102Integration();
                    string data = dt.Get_ANCDetails("KCRKIT", "102@KCRKIT");
                    string updatestmt = "update wssyncstatus set status ='Processing', lastcheckdate = now(),currentStatus = 'GetANCDetails' where isactive=1;";
                    helper.executeInsertStatement(updatestmt);
                    helper.TraceService_result(data);
                    DataTable dta = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));
                    if (dta == null || dta.Rows.Count <= 0)
                    {
                        helper.TraceService_abnormal("GetANCDetails --Count is less than or equal to zero");
                        helper.pushPostEventDetails(dtWsSyncDetails);
                    }
                    else
                    {
                        int count = dta.Rows.Count;
                        int count2 = Convert.ToInt32(dta.Rows[count - 1]["Count"].ToString());
                        if (count2 == 0)
                        {
                            helper.TraceService_abnormal("GetANCDetails -- Count is or equal to zero");
                            helper.pushPostEventDetails(dtWsSyncDetails);
                        }
                        if (count == count2 + 1)
                        {
                            helper.TraceService("counts matched for batch id:" + dta.Rows[count - 1]["BatchId"].ToString());
                            helper.InsertUpdateANCDetails(dta, count);
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
                        GetANCDetails(dtWsSyncDetails);
                    }
                }
                catch (Exception ex)
                {
                    helper.TraceService_abnormal(ex.ToString());
                }
            }
            else
            {
                helper.pushPostEventDetails(dtWsSyncDetails);
            }
        }
    }
}
