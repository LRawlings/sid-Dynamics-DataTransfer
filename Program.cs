using System.Collections.Generic;
using FileHelpers; // library for reading csv files

namespace SID_DOT_NET
{
    class Program
    {
        // Define the layout of the SID Case table export (CMM)
        [DelimitedRecord("|")]
        public class CASE
        {
            public string GRP_NAME1; // Case Category
            public string GRP_NAME2; // Sub Catagory Name
            public string CMM_CCNC; // Student Code (Contact in D365)
            public string CMM_CUSC; // Created By
            public string SID_Owner; // Owner
            public string CMM_CRED; // Created On
            public string CMM_UPDU; // Modified By
            public string CMM_UPDD; // Modified On
            public string CMM_CODE; // SID Case Code
            public string CMM_CLSD; // Status
        }

        // Define the layout of the SID_Student_ID/D365_Student_ID lookup file
        [DelimitedRecord(",")]
        public class STUD_ID
        {
            public string D365_ID;
            public string SID_ID;
        }

        // Define the layout of the SID_Staff_ID/D365_Staff_ID lookup file
        [DelimitedRecord(",")]
        public class STAFF_ID
        {
            public string D365_ID;
            public string SID_ID;
        }

        static void Main(string[] args)
        {
            // Define engine for reading the SID Case table export (CMM) file
            var engine = new FileHelperEngine<CASE>();
            // Add contents of SID Case table export (CMM) to array
            var sid_export = engine.ReadFile("export.dsv");
            // Define list to be populated after student ID conversion
            var converted = new List<CASE>();

            // Define engine for reading the SID_Student_ID/D365_Student_ID lookup file
            var engine2 = new FileHelperEngine<STUD_ID>();
            // Add contents of SID_Student_ID/D365_Student_ID lookup file to array
            var id_lookup = engine2.ReadFile("student_id_lookup.csv");

            // Define engine for reading the SID_Staff_ID/D365_Staff_ID lookup file
            var engine3 = new FileHelperEngine<STAFF_ID>();
            // Add contents of SID_Staff_ID/D365_Staff_ID lookup file to array
            var id_lookup2 = engine3.ReadFile("staff_id_lookup.csv");

            // Loop through each line of the SID Case table export (CMM)
            foreach (CASE N in sid_export)
            {
                // Reset the d365_code string (student)
                string d365_code = "";
                // Loop through each line of the SID_Student_ID/D365_Student_ID lookup file 
                foreach (STUD_ID M in id_lookup)
                {
                    // When it find the SID_Student_Code, save the corresponding D365 code to the d365_code string
                    if (M.SID_ID.ToString() == N.CMM_CCNC.ToString())
                    {
                        d365_code = M.D365_ID;
                    }
                }
                if (N.CMM_CCNC == "Student")
                {
                    d365_code = "Student";
                }

                // Reset the d365_code string (staff created by)
                string d365_code_staff = "";
                // Reset the d365_code string (staff modified by)
                string d365_code_staff2 = "";
                // Reset the d365_code string (owner)
                string d365_code_staff3 = "";
                // Loop through each line of the SID_Staff_ID/D365_Staff_ID lookup file 
                foreach (STAFF_ID M in id_lookup2)
                {
                    // When it finds the SID_Staff_Code, save the corresponding D365 code to the d365_code_staff string (CMM_CUSC)
                    if (M.SID_ID.ToString().ToUpper() == N.CMM_CUSC.ToString().ToUpper())
                    {
                        d365_code_staff = M.D365_ID;
                    }
                    // When it finds the SID_Staff_Code, save the corresponding D365 code to the d365_code_staff2 string (CMM_UPDU)
                    if (M.SID_ID.ToString().ToUpper() == N.CMM_UPDU.ToString().ToUpper())
                    {
                        d365_code_staff2 = M.D365_ID;
                    }
                    // When it finds the SID_Staff_Code, save the corresponding D365 code to the d365_code_staff3 string (SID_Owner)
                    if (M.SID_ID.ToString().ToUpper() == N.SID_Owner.ToString().ToUpper())
                    {
                        d365_code_staff3 = M.D365_ID;
                    }
                }
                // Override the lookup for the header line
                if (N.CMM_CUSC == "SID Created By")
                {
                    d365_code_staff = "SID Created By";
                }
                if (N.CMM_UPDU == "SID Modified By")
                {
                    d365_code_staff2 = "SID Modified By";
                }
                if (N.SID_Owner == "SID Owner")
                {
                    d365_code_staff3 = "SID Owner";
                }
                // Override the owner lookup where the owner is set to 'MULTIPLE'
                if (N.SID_Owner == "MULTIPLE")
                {
                    d365_code_staff3 = "MULTIPLE";
                }

                // Add this entry to the 'converted' list including the converted student code
                converted.Add(new CASE()
                {
                    GRP_NAME1 = N.GRP_NAME1,
                    GRP_NAME2 = N.GRP_NAME2,
                    CMM_CCNC = d365_code, // Note. we are using the d365_code instead of the SID code we passed in
                    CMM_CUSC = d365_code_staff,
                    SID_Owner = d365_code_staff3,
                    CMM_CRED = N.CMM_CRED,
                    CMM_UPDU = d365_code_staff2,
                    CMM_UPDD = N.CMM_UPDD,
                    CMM_CODE = N.CMM_CODE,
                    CMM_CLSD = N.CMM_CLSD
                });
            }

            // Write to file
            engine.WriteFile("output.txt", converted);
        }
    }
}