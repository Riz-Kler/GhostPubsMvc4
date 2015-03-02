 
 ;
    WITH cteHauntedPubs
    AS (
    -- HauntedOrgs
    SELECT
      o.*
    FROM Organisation.Note AS o WITH (NOLOCK)
    --WHERE (HauntedStatus = 1)),
	)
    , cteHauntedPubBuildings
    AS (SELECT
      *,
	ROW_NUMBER() OVER (PARTITION BY orgid ORDER BY orgid) AS [Sequence]
    FROM cteHauntedPubs o WITH (NOLOCK)
	)
    
	SELECT
      *
    FROM cteHauntedPubBuildings WITH (NOLOCK)
	order by orgid, LastModified desc, CreateDate desc
    -- WHERE [Sequence] = 1
	 