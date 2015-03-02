 
update Organisation.Note 
set text = 'HAUNTED: ' + text 
FROM            Organisation.Note AS n WITH (NOLOCK) INNER JOIN
                         Organisation.Org AS o ON o.ID = n.OrgID
WHERE        (o.HauntedStatus = 1) AND (NOT (n.Text LIKE 'haunted%'))
 