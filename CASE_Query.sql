set define off
SELECT
    CASE TO_CHAR(GRP.GRP_NAME)
		WHEN 'Consent to Share Withheld' THEN 'Helpdesk enquiries'
		WHEN 'Appeals' THEN 'Director Student Services'
		WHEN 'Council Tax' THEN 'Student money Advice'
		WHEN 'Concerns' THEN 'Director Student Services'
		WHEN 'Short Term Loans' THEN 'Student money Advice'
		WHEN 'Placement Year - Student Funding & Finance' THEN 'Student money Advice'
		WHEN 'Diagnostic Test' THEN 'Student money Advice'
		WHEN 'Care Leavers' THEN 'Student money Advice'
		WHEN 'Bursaries' THEN 'Student money Advice'
		WHEN 'Postgraduate' THEN 'Student money Advice'
		WHEN 'Hardship Funds' THEN 'Student money Advice'
		WHEN '4321' THEN 'Director Student Services'
		WHEN 'Helpdesk' THEN 'Helpdesk enquiries'
		WHEN 'Return to Study' THEN 'Helpdesk enquiries'
		WHEN 'Shared Case' THEN 'Cross-Team Cases'
		ELSE TO_CHAR(GRP.GRP_NAME) END AS "Case Category", -- Case Category
    CASE TO_CHAR(GRP2.GRP_NAME)
		WHEN TO_CHAR(GRP.GRP_NAME) THEN ''
		WHEN 'Pre-entry IMCs' THEN 'Pre-entry IMC'
		WHEN 'Consent to Share Withheld' THEN 'Consent to share withheld'
		WHEN 'Consent to Share Granted' THEN 'Consent to share granted'
		WHEN 'working with risk' THEN 'Working with risk'
		WHEN 'Return to Study' THEN 'Helpdesk return to study'
		WHEN 'Helpdesk' THEN 'Helpdesk enquiries'
		WHEN 'SMART Return to Study' THEN 'SMA Return to Study'
		ELSE TO_CHAR(GRP2.GRP_NAME)
		END AS "Case Subcategory", -- Sub Catagory Name
    CMM.CMM_CCNC AS "Student", -- Student Code (Student in D365)
    CMM.CMM_CUSC AS "SID Created By", -- Created By
    CASE WHEN TO_CHAR(CMR2.CMR_CUSC) LIKE ('%;%') THEN 'MULTIPLE'
        ELSE TO_CHAR(CMR2.CMR_CUSC)
        END AS "SID Owner", -- Owner
    TO_CHAR(CMM.CMM_CRED, 'dd/mm/yyyy hh24:mi') AS "SID Created On", -- Created On
    CMM.CMM_UPDU AS "SID Modified By", -- Modified By
    to_char(CMM.CMM_UPDD, 'dd/mm/yyyy hh24:mi') AS "SID Modified On", -- Modified On
    CMM.CMM_CODE AS "SID Case Code", -- SID Case Code    CASE TO_CHAR(CMM.CMM_CLSD) WHEN 'Y' THEN 'On Hold' WHEN 'N' THEN 'In Progress' END AS "Status Reason" -- Status (converted to D365 format - However we are using on hold instead of resolved as D365 wont let you import as resolved)

FROM
    CMM CMM -- Case Table
    LEFT JOIN GRP GRP ON GRP.GRP_CODE = CMM.CMM_GRPC -- Catagory Table
    LEFT JOIN GRP GRP2 ON GRP2.GRP_CODE = CMM.CMM_GRP2 -- Catagory Table (sub-catagory)
    LEFT JOIN CMC CMC ON CMC.CMC_CODE = CMM.CMM_CMCC -- Case Management Causes Table

    -- Join to Owners table and create a semi-colon delimited list of the owners
    LEFT JOIN (SELECT listagg(TO_CHAR(CMR.CMR_CUSC),';') within group(order by CMR.CMR_CUSC) AS "CMR_CUSC", CMR.CMR_CMMC FROM ESD_CMR CMR
        -- Only inlcude for these specific users
        WHERE CMR.CMR_CUSC IN ('','','') AND CMR.CMR_DELT = 'N' GROUP BY CMR.CMR_CMMC) CMR2 ON CMR2.CMR_CMMC = CMM.CMM_CODE -- Owner(s)

WHERE

    -- Exclude any Cases which have been deleted
    CMM.CMM_DELT = 'N'

    -- Only include Cases which are owned by specific users
    AND CMM.CMM_CODE IN (
        SELECT CMM_CODE FROM CMM CMM INNER JOIN ESD_CMR CMR ON CMR.CMR_CMMC = CMM.CMM_CODE
            AND CMR.CMR_CUSC IN ('','','')AND CMR.CMR_DELT = 'N')

    -- Exclude Cases which are also owned by other specific users
    AND CMM.CMM_CODE NOT IN (
        SELECT CMM.CMM_CODE FROM CMM CMM INNER JOIN ESD_CMR CMR ON CMR.CMR_CMMC = CMM.CMM_CODE
        AND CMR.CMR_CUSC IN ('','','') AND CMR.CMR_DELT = 'N')

ORDER BY
    CMM.CMM_CODE DESC;