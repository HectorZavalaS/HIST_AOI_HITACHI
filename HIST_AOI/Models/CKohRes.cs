using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIST_AOI.Models
{
    public class CKohRes
    {
        int m_PCBResultBefore;
        int m_PCBResultAfter;
        String m_BarCode;
        int m_Checksum;

        public int PCBResultBefore { get => m_PCBResultBefore; set => m_PCBResultBefore = value; }
        public int PCBResultAfter { get => m_PCBResultAfter; set => m_PCBResultAfter = value; }
        public string BarCode { get => m_BarCode; set => m_BarCode = value; }
        public int Checksum { get => m_Checksum; set => m_Checksum = value; }
    }
}
