# sid-Dynamics-DataTransfer
SQL Querie to get data out of the SID Oracle DB then .Net app to convert old GUID to Dynamics GUID and then import in to Dynamics 365.
## Step 1 – Download the C# app

## Step 2 - Run SQL Query and export the results

1. Copy the contents of the CASE_Query_JAN_ONLY.sql file (in this folder) to a SQL Developer Worksheet

2. Amend line 186 and 196 to only include the cases for specific users and line 201 to exclude cases for specific user, then run query - Ensure you select the first line (set define off) as well as the entire query before you run or it will try and use the ampersands as inputs

3. This will take a bit of time due to the number of joins to the Case Additional fields table.

4. Once the query has run, right click on the results and Export.

5. Choose the following options:

   a. Format: delimited

   b. Header: Ticked

   c. Delimiter: |

   d. Left Enclosure: None

   e. Right Enclosure: None

   f. File: inside the folder extracted in step 1 and named export.dsv

6. Next

7. Finish

8. This will take a few seconds to extract

## Step 3 – Create the Student and Staff ID Lookup files

1. Inside the CASE_APP folder, there is a file named student_id_lookup.csv

2. This file needs to be populated with the OLD GUID (student code) on the right and the equivalent Dynamics 365 customer ID on the left

3. In Dynamics, open Advanced Find

4. In Look For, select Contacts

5. Select Edit Columns

6. Remove all columns

7. Add Student Code and click OK

8. Leave Use Saved View as [new] and click Results

9. Under Data, Select Export Contacts > Static Worksheet

10. Save this file and open

11. The SID Student Code will be in column D and the Dynamics UID will be in Column A (you will need to expand the columns to view as they are hidden by default)

12. Copy these values to the student_id_lookup.csv file and save. Ensure you don't loose any leading 0's which Excel will try it's best to remove!

13. Repeate for the Staff Ids in a file names staff_id_lookup.csv

## Step 4 – Convert the IDs in the SQL export file

1. Make sure you have .NET Core SDK Installed

2. Open Command Prompt/PowerShell

3. Change directory to the CASE_APP folder extracted earlier

4. In the Terminal window type dotnet run and enter

5. This should produce a file in the CASE_APP folder named output.txt

## Step 5 – Open the converted file in excel (note these notes are for the latest version of Excel Office 365)

1. Open excel > Blank workbook

2. Select Data tab from the ribbon

3. From Text/CSV

4. Find the output.txt file and import

5. Transform Data

6. Transform Tab

7. Check the preview of the data has determined columns and headers OK

8. If the columns are not set: Split Column > By Delimiter > Custom > |

9. If the Headers are not set: Select 'Use First Row as Headers'

10. Select the SID Case Code column header: Data Type > Text (this is to stop excel dropping the leading 0

11. Select the 6 Wise Choices column headers: Data Type > Text (this is to stop Excel trning them in to dates)

12. Close & Load

13. Save file in a temp location

14. Open the Case_Template.xlsx file

15. Copy the data over from the previous file (the columns should match up excluding the 3 hidden columns in the template)

16. Save the filled in template with a sensible name (including the date)

## Step 6 – Import in to Dynamics 365

1. Settings > Data Management > Imports > Import Data

2. Select the filled in template file and click Next

3. If there are obvious mapping issues, it will show up now. If not, you will reach the Review screen

4. Ensure Allow Duplicates is set to No and click Submit

5. Click Finish and you should see your import listed under My Imports.

6. You can click refresh (top right) and watch it progress through the various steps.

7. Depending on how many records, this can take a long time to process

8. Once complete, you can check any errors by double clicking the import and selecting the Failures tab. 
