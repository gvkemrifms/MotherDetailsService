using Newtonsoft.Json;
using System;
using System.Data;

namespace getMotherDetails
{
    public class PregnencyDetails
    {
        private readonly HelperClass _helper = new HelperClass();
        readonly AncDetails _ancDetails = new AncDetails();
        public void GetPregancyDetails(DataTable dtWsSyncDetails)
        {
            if (dtWsSyncDetails == null) throw new ArgumentNullException(nameof(dtWsSyncDetails));
            if (dtWsSyncDetails.Rows[0]["GetPregancyDetails"].ToString() == "1")
            {
                try
                {
                    var dt = new GVK_PregancyDetails._102Integration();
                    var data = dt.Get_PregancyDetails("KCRKIT", "102@KCRKIT");
                    _helper.TraceService_result(data);
                    string updatestmt = "update wssyncstatus set status ='Processing', lastcheckdate = now(),currentStatus = 'GetPregancyDetails' where isactive=1;";
                    _helper.ExecuteInsertStatement(updatestmt);
                    var dta = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));
                    if (dta == null || dta.Rows.Count <= 0)
                    {
                        _helper.TraceService_abnormal("GetPregancyDetails -- Count is less than or equal to zero");
                        _ancDetails.GetAncDetails(dtWsSyncDetails);
                    }
                    else
                    {
                        int count = dta.Rows.Count;
                        int count2 = Convert.ToInt32(dta.Rows[count - 1]["Count"].ToString());

                        if (count2 == 0)
                        {
                            _helper.TraceService_abnormal("GetPregnancyDetails -- Count is or equal to zero");
                            _ancDetails.GetAncDetails(dtWsSyncDetails);
                        }
                        if (count == count2 + 1)
                        {
                            _helper.TraceService("GetPregancyDetails -- counts matched for batch id:" + dta.Rows[count - 1]["BatchId"].ToString());
                            _helper.InsertUpdatePregencyDetails(dta, count);
                            GVK_UPDATE_SyncDetails._102Integration us = new GVK_UPDATE_SyncDetails._102Integration();
                            string res = us.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "1");
                            _helper.TraceService("Response after updation for batch id :::" + dta.Rows[count - 1]["BatchId"].ToString() + ":::" + res);

                        }
                        else
                        {
                            _helper.TraceService_abnormal("Counts are not matched for the batch with batch id " + dta.Rows[count - 1]["BatchId"].ToString());
                            var us = new GVK_UPDATE_SyncDetails._102Integration();
                            string res = us.Update_SyncDetails("KCRKIT", "102@KCRKIT", dta.Rows[count - 1]["BatchId"].ToString(), System.DateTime.Now.ToString(), "0");
                            _helper.TraceService("Response after updation for failed batch id :::" + dta.Rows[count - 1]["BatchId"].ToString() + ":::" + res);

                        }
                        GetPregancyDetails(dtWsSyncDetails);
                    }
                }
                catch (Exception ex)
                {
                    _helper.TraceService_abnormal(ex.ToString());
                }
            }
            else
            {
                _ancDetails.GetAncDetails(dtWsSyncDetails);
            }
        }
    }
}
