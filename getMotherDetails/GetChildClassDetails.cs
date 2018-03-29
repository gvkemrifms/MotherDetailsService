using Newtonsoft.Json;
using System;
using System.Data;

namespace getMotherDetails
{
    public class GetChildClassDetails
    {
        private readonly HelperClass _helper = new HelperClass();
        private readonly PregnencyDetails _pregnencyDetails = new PregnencyDetails();
        public void GetChildDetails(DataTable dtWsSyncDetails)
        {
            if (dtWsSyncDetails == null) throw new ArgumentNullException(nameof(dtWsSyncDetails));

            if (dtWsSyncDetails.Rows[0]["GetChildDetails"].ToString() == "1")
            {
                try
                {
                    var dt = new GVK_ChildDetails._102Integration();

                    var data = dt.Get_ChildDetails("KCRKIT", "102@KCRKIT");
                    var updatestmt = "update wssyncstatus set status ='Processing', lastcheckdate = now(),currentStatus = 'GetChildDetails' where isactive=1;";
                    _helper.ExecuteInsertStatement(updatestmt);
                    _helper.TraceService_result(data);
                    var dta = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));

                    if (dta == null || dta.Rows.Count <= 0)
                    {
                        _helper.TraceService_abnormal("GetChildDetails is less than or equal to zero");
                        _pregnencyDetails.GetPregancyDetails(dtWsSyncDetails);
                    }
                    else
                    {
                        int count = dta.Rows.Count;
                        int count2 = Convert.ToInt32(dta.Rows[count - 1]["Count"].ToString());
                        if (count2 == 0)
                        {
                            _helper.TraceService_abnormal("GETChildDETAILS -- Count is or equal to zero");
                            _pregnencyDetails.GetPregancyDetails(dtWsSyncDetails);
                        }
                        if (count == count2 + 1)
                        {
                            _helper.TraceService("GetChildDetails -- counts matched for batch id:" + dta.Rows[count - 1]["BatchId"].ToString());
                            _helper.InsertUpdateChildDetails(dta, count);
                            var us = new GVK_UPDATE_SyncDetails._102Integration();
                            string res = us.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "1");
                            _helper.TraceService("Response after updation for batch id :::" + dta.Rows[count - 1]["BatchId"] + ":::" + res);
                            
                        }
                        else
                        {
                            _helper.TraceService_abnormal("Counts are not matched for the batch with batch id " + dta.Rows[count - 1]["BatchId"].ToString());
                            var us = new GVK_UPDATE_SyncDetails._102Integration();
                            string res = us.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), System.DateTime.Now.ToString(), "0");
                            _helper.TraceService("Response after updation for failed batch id :::" + dta.Rows[count - 1]["BatchId"].ToString() + ":::" + res);

                        }
                        GetChildDetails(dtWsSyncDetails);
                    }
                }
                catch (Exception ex)
                {
                    _helper.TraceService_abnormal(ex.ToString());
                }
            }
            else
            {
                _pregnencyDetails.GetPregancyDetails(dtWsSyncDetails);
            }
        }
    }
}
