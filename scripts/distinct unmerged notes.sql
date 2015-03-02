USE CMS

;
WITH cteHauntedPubs
AS (
-- HauntedOrgs
-- Notes
SELECT
  n.*
FROM Organisation.Note AS n WITH (NOLOCK)
INNER JOIN Organisation.Org AS o WITH (NOLOCK)
  ON o.id = n.OrgID
WHERE (o.HauntedStatus = 1)),
cteHauntedPubBuildings
AS (SELECT
  *,
  ROW_NUMBER() OVER (PARTITION BY orgid ORDER BY orgid) AS [Sequence]
FROM cteHauntedPubs o WITH (NOLOCK))

SELECT
  *
FROM cteHauntedPubBuildings WITH (NOLOCK)
WHERE [Sequence] =2 
ORDER BY orgid, LastModified DESC, CreateDate DESC
