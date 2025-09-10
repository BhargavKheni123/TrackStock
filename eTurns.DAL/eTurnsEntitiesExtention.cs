using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Objects.DataClasses;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Reflection;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;


namespace eTurns.DAL
{
    #region Contexts

    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class eTurnsEntities
    {
        
        public Int32 UserID { get; set; }

        List<DBAudit> auditTrailList = new List<DBAudit>();

        public enum AuditActions
        {
            I,
            U,
            D
        }

        partial void OnContextCreated()
        {
            this.SavingChanges += new EventHandler(eTurnsEntities_SavingChanges);
        }

        void eTurnsEntities_SavingChanges(object sender, EventArgs e)
        {
            IEnumerable<ObjectStateEntry> changes = this.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified);

            if (changes.Where(t => t.EntitySet.Name != "UsersUISettings" && t.EntitySet.Name != "GlobalUISettings").Count() > 0)
            {
                foreach (ObjectStateEntry stateEntryEntity in changes)
                {
                    if (!stateEntryEntity.IsRelationship &&
                            stateEntryEntity.Entity != null &&
                                !(stateEntryEntity.Entity is DBAudit))
                    {//is a normal entry, not a relationship
                        DBAudit audit = this.AuditTrailFactory(stateEntryEntity, UserID);
                        auditTrailList.Add(audit);
                    }
                }

                if (auditTrailList.Count > 0)
                {
                    foreach (var audit in auditTrailList)
                    {//add all audits 
                        this.AddToDBAudits(audit);
                    }
                }
            }
        }

        private DBAudit AuditTrailFactory(ObjectStateEntry entry, Int32 UserID)
        {
            DBAudit audit = new DBAudit();
            audit.AuditId = Guid.NewGuid().ToString();
            audit.RevisionStamp = DateTime.Now;
            audit.TableName = entry.EntitySet.Name;
            audit.UserID = UserID;

            if (entry.State == EntityState.Added)
            {//entry is Added 
                audit.NewData = GetEntryValueInString(entry, false);
                audit.Actions = AuditActions.I.ToString();
            }
            else if (entry.State == EntityState.Deleted)
            {//entry in deleted
                audit.OldData = GetEntryValueInString(entry, true);
                audit.Actions = AuditActions.D.ToString();
            }
            else
            {//entry is modified
                audit.OldData = GetEntryValueInString(entry, true);
                audit.NewData = GetEntryValueInString(entry, false);
                audit.Actions = AuditActions.U.ToString();

                IEnumerable<string> modifiedProperties = entry.GetModifiedProperties();
                //assing collection of mismatched Columns name as serialized string 
                audit.ChangedColumns = XMLSerializationHelper.XmlSerialize(modifiedProperties.ToArray());
            }

            return audit;
        }

        private string GetEntryValueInString(ObjectStateEntry entry, bool isOrginal)
        {
            if (entry.Entity is EntityObject)
            {
                object target = CloneEntity((EntityObject)entry.Entity);
                foreach (string propName in entry.GetModifiedProperties())
                {
                    object setterValue = null;
                    if (isOrginal)
                    {
                        //Get orginal value 
                        setterValue = entry.OriginalValues[propName];
                    }
                    else
                    {
                        //Get orginal value 
                        setterValue = entry.CurrentValues[propName];
                    }
                    //Find property to update 
                    PropertyInfo propInfo = target.GetType().GetProperty(propName);
                    //update property with orgibal value 
                    if (setterValue == DBNull.Value)
                    {//
                        setterValue = null;
                    }
                    propInfo.SetValue(target, setterValue, null);
                }//end foreach

                XmlSerializer formatter = new XmlSerializer(target.GetType());
                XDocument document = new XDocument();

                using (XmlWriter xmlWriter = document.CreateWriter())
                {
                    formatter.Serialize(xmlWriter, target);
                }
                return document.Root.ToString();
            }
            return null;
        }

        public EntityObject CloneEntity(EntityObject obj)
        {
            DataContractSerializer dcSer = new DataContractSerializer(obj.GetType());
            MemoryStream memoryStream = new MemoryStream();

            dcSer.WriteObject(memoryStream, obj);
            memoryStream.Position = 0;

            EntityObject newObject = (EntityObject)dcSer.ReadObject(memoryStream);
            return newObject;
        }

    }
    #endregion
}