 use CMS

 ;
    WITH cteNotes
    AS (
    -- HauntedOrgs
    SELECT
      n.*
    FROM Organisation.Note AS n WITH (NOLOCK)
	join Organisation.Org o
		on o.ID = n.OrgID
	WHERE (o.HauntedStatus = 1)
	)
    , cteHaunted
    AS (SELECT
      *,
	ROW_NUMBER() OVER (PARTITION BY orgid ORDER BY orgid) AS [Sequence]
    FROM cteNotes n WITH (NOLOCK)
	)
    
	SELECT
      *
    FROM cteHaunted WITH (NOLOCK)
	order by orgid, LastModified desc, CreateDate desc
    -- WHERE [Sequence] = 1
	 
 