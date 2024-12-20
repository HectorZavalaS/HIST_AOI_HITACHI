using HIST_AOI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIST_AOI.Class
{
    public class CKoh_Young
    {
        KY_AOIEntities m_KYDb;
        CKohRes m_aoi_results;

        public CKoh_Young()
        {
            m_KYDb = new KY_AOIEntities();
            m_aoi_results = new CKohRes();
        }
        public bool getAOIRes(String serial)
        {
            bool result = false;

            var AOIResults = m_KYDb.TB_AOIPCB.Where(res => res.BarCode == serial).Select(res => new { PCBResultBefore = res.PCBResultBefore, PCBResultAfter = res.PCBResultAfter, Checksum = res.Checksum, Barcode = res.BarCode});

            foreach(var item in AOIResults)
            {
                if(item.PCBResultBefore == 13000000 && item.Checksum == 1)
                {
                    if (item.PCBResultAfter == 12000000 || item.PCBResultAfter == 11000000)
                        result = true;
                    else result = false;
                }
                else
                {
                    if (item.PCBResultBefore == 11000000 && item.PCBResultAfter == 11000000)
                        result = true;
                    else
                    {
                        if (item.PCBResultBefore == 12000000 && item.PCBResultAfter == 12000000)
                            result = true;
                        else result = false;
                    }
                    
                }
            }

            return result;
        }
    }
}
