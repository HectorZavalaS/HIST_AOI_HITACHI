using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIST_AOI.Class
{
    public class CGeikenResult
    {
        String m_serial_number;
        String m_program_name;
        String m_side;
        String m_neo_result;
        String m_neo_name;
        String m_neo_user;
        DateTime m_neo_date_test;
        String m_aoi_result;
        String m_cod_ref_cmp;
        String m_aoi_defect;
        String m_aoi_line;
        String m_defect_name;
        DateTime m_date_aoi_test;
        String m_new_result_test;
        DateTime m_date_new_test;

        public string Serial_number { get => m_serial_number; set => m_serial_number = value; }
        public string Program_name { get => m_program_name; set => m_program_name = value; }
        public string Aoi_result { get => m_aoi_result; set => m_aoi_result = value; }
        public string Neo_result { get => m_neo_result; set => m_neo_result = value; }
        public string Cod_ref_cmp { get => m_cod_ref_cmp; set => m_cod_ref_cmp = value; }
        public string Aoi_defect { get => m_aoi_defect; set => m_aoi_defect = value; }
        public string Defect_name { get => m_defect_name; set => m_defect_name = value; }
        public string New_result_test { get => m_new_result_test; set => m_new_result_test = value; }
        public DateTime Date_aoi_test { get => m_date_aoi_test; set => m_date_aoi_test = value; }
        public DateTime Date_neo_test { get => m_neo_date_test; set => m_neo_date_test = value; }
        public DateTime Date_new_test { get => m_date_new_test; set => m_date_new_test = value; }
        public string Side { get => m_side; set => m_side = value; }
        public string Aoi_name { get => m_aoi_line; set => m_aoi_line = value; }
        public string Neo_name { get => m_neo_name; set => m_neo_name = value; }
        public string Neo_log_name { get => m_neo_user; set => m_neo_user = value; }
    }
}
