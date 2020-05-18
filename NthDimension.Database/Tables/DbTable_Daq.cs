using System.Data;
using System.Xml.Serialization;

namespace NthDimension.Database.Tables
{
    /// <summary>
    /// The Schema for data acquisition data tables
    /// </summary>
    public class DbTable_Daq : DataTable, IXmlSerializable
    {
        //protected DataRowCollection m_dataRows;         // ToDo:: Is that really required? Can I get by with DataTable.Rows?
        public DbTable_Daq()
            : this("TableDaq")
        {

        }

        public DbTable_Daq(string name)
        {
            this.TableName = name;

            #region Columns
            this.Columns.Add("Description", typeof(string));                    // 0
            this.Columns.Add("Engineering Unit", typeof(string));               // 1
            this.Columns.Add("Tag Name", typeof(string));                       // 2
            this.Columns.Add("Current Value", typeof(string));                  // 3
            this.Columns.Add("Quality", typeof(string));                        // 4
            this.Columns.Add("Timestamp", typeof(string));                      // 5
            this.Columns.Add("1 Min", typeof(string));                          // 6
            this.Columns.Add("5 Min", typeof(string));                          // 7
            this.Columns.Add("15 Min", typeof(string));                         // 8
            this.Columns.Add("30 Min", typeof(string));                         // 9
            this.Columns.Add("60 Min", typeof(string));                         // 10
            this.Columns.Add("Integrity", typeof(string));                      // 11
            this.Columns.Add("Minimum", typeof(string));                        // 12
            this.Columns.Add("Maximum", typeof(string));                        // 13
            this.Columns.Add("Default Value", typeof(string));                  // 14
            this.Columns.Add("On-Line", typeof(string));                        // 15
            this.Columns.Add("Enabled", typeof(string));                        // 16   
            this.Columns.Add("Record", typeof(string));                         // 17
            this.Columns.Add("Read Format", typeof(string));                    // 18
            this.Columns.Add("Write Format", typeof(string));                   // 19
            this.Columns.Add("Destination File", typeof(string));              // 20
            this.Columns.Add("Destination Record", typeof(string));            // 21
            this.Columns.Add("Destination Format", typeof(string));            // 22
            this.Columns.Add("Source Type", typeof(string));                   // 23
            this.Columns.Add("Source Value", typeof(string));               // 24
            #endregion

            #region DBNull
            this.Columns[0].AllowDBNull = true;
            this.Columns[1].AllowDBNull = true;
            this.Columns[2].AllowDBNull = true;
            this.Columns[3].AllowDBNull = true;
            this.Columns[4].AllowDBNull = true;
            this.Columns[5].AllowDBNull = true;
            this.Columns[6].AllowDBNull = true;
            this.Columns[7].AllowDBNull = true;
            this.Columns[8].AllowDBNull = true;
            this.Columns[9].AllowDBNull = true;
            this.Columns[10].AllowDBNull = true;
            this.Columns[11].AllowDBNull = true;
            this.Columns[12].AllowDBNull = true;
            this.Columns[13].AllowDBNull = true;
            this.Columns[14].AllowDBNull = true;
            this.Columns[15].AllowDBNull = true;
            this.Columns[16].AllowDBNull = true;
            this.Columns[17].AllowDBNull = true;
            this.Columns[18].AllowDBNull = true;
            this.Columns[19].AllowDBNull = true;
            #endregion

            #region // commented out
            //this.Columns[0].MaxLength = -1;
            //this.Columns[1].MaxLength = -1;
            //this.Columns[2].MaxLength = -1;
            //this.Columns[3].MaxLength = -1;
            //this.Columns[4].MaxLength = -1;
            //this.Columns[5].MaxLength = -1;
            //this.Columns[6].MaxLength = -1;
            //this.Columns[7].MaxLength = -1;
            //this.Columns[8].MaxLength = -1;
            //this.Columns[9].MaxLength = -1;
            //this.Columns[10].MaxLength = -1;
            //this.Columns[11].MaxLength = -1;
            //this.Columns[12].MaxLength = -1;
            //this.Columns[13].MaxLength = -1;
            //this.Columns[14].MaxLength = -1;
            //this.Columns[15].MaxLength = -1;
            //this.Columns[16].MaxLength = -1;
            //this.Columns[17].MaxLength = -1;
            //this.Columns[18].MaxLength = -1;
            //this.Columns[19].MaxLength = -1;

            //this.PrimaryKey = new DataColumn[1] { this.Columns[0] };
            #endregion

        }
    }
}
