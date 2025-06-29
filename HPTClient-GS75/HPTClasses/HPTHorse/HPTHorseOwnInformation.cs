using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseOwnInformationCollection : Notifier
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTHorseOwnInformation> HorseOwnInformationList { get; set; }

        internal void SaveHorseOwnInformationList()
        {
            string fileName = HPTConfig.MyDocumentsPath + "HorseOwnInformationList.hptinfo";
            HPTSerializer.SerializeHPTHorseOwnInformation(fileName, this);
        }
        
        internal HPTHorseOwnInformation MergeHorseOwnInformation(HPTHorse horse)
        {
            try
            {
                if (this.HorseOwnInformationList == null)
                {
                    this.HorseOwnInformationList = new ObservableCollection<HPTHorseOwnInformation>();
                }
                var savedHorseInformation = this.HorseOwnInformationList.FirstOrDefault(oi => oi.Name == horse.HorseName);
                if (horse.OwnInformation == savedHorseInformation)
                {
                    return horse.OwnInformation;
                }
                if (savedHorseInformation == null && horse.OwnInformation != null)
                {
                    this.HorseOwnInformationList.Add(horse.OwnInformation);
                    savedHorseInformation = horse.OwnInformation;
                    if (horse.OwnInformation.HorseOwnInformationCommentList.Count > 0)
                    {
                        horse.OwnInformation.HasComment = true;
                    }
                    return horse.OwnInformation;
                }
                else if (horse.OwnInformation != null)
                {
                    if (savedHorseInformation.HorseOwnInformationCommentList == null || savedHorseInformation.HorseOwnInformationCommentList.Count == 0)
                    {
                        savedHorseInformation.HorseOwnInformationCommentList = new ObservableCollection<HPTHorseOwnInformationComment>(horse.OwnInformation.HorseOwnInformationCommentList);
                    }
                    else
                    {
                        foreach (var ownComment in horse.OwnInformation.HorseOwnInformationCommentList)
                        {
                            var existingComment = savedHorseInformation.HorseOwnInformationCommentList.FirstOrDefault(oic => oic.Comment == savedHorseInformation.Comment);
                            if (existingComment == null)
                            {
                                savedHorseInformation.HorseOwnInformationCommentList.Add(ownComment);
                            }
                        }
                    }
                }
                if (savedHorseInformation != null && savedHorseInformation.HorseOwnInformationCommentList.Count > 0)
                {
                    savedHorseInformation.HasComment = true;
                }

                // Spara filen efter varje ändring
                ThreadPool.QueueUserWorkItem(new WaitCallback(SaveHorseOwnInformationList), ThreadPriority.Normal);

                return savedHorseInformation;
            }
            catch (Exception exc)
            {
                return horse.OwnInformation;
            }
        }

        internal void SaveHorseOwnInformationList(object state)
        {
            try
            {
                SaveHorseOwnInformationList();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        internal HPTHorseOwnInformation GetOwnInformationByName(string name)
        {
            if (this.HorseOwnInformationList == null)
            {
                this.HorseOwnInformationList = new ObservableCollection<HPTHorseOwnInformation>();
            }

            var ownInformation = this.HorseOwnInformationList.FirstOrDefault(oi => oi != null && oi.Name != null && oi.Name == name);
            if (ownInformation != null)
            {
                if (string.IsNullOrEmpty(ownInformation.Comment) 
                    && (ownInformation.HorseOwnInformationCommentList == null || ownInformation.HorseOwnInformationCommentList.Count == 0)
                    && (ownInformation.NextTimer == null || ownInformation.NextTimer == false))
                {
                    try
                    {
                        this.HorseOwnInformationList.Remove(ownInformation);
                    }
                    catch (Exception exc)
                    {
                        string s = exc.Message;
                    }
                    ownInformation = null;
                    return null;
                }
            }
            else
            {
                return null;
            }

            if (!string.IsNullOrEmpty(ownInformation.Comment))
            {
                if (ownInformation.HorseOwnInformationCommentList == null || ownInformation.HorseOwnInformationCommentList.Count == 0)
                {
                    ownInformation.HorseOwnInformationCommentList = new ObservableCollection<HPTHorseOwnInformationComment>();
                    var ownInformationComment = new HPTHorseOwnInformationComment()
                    {
                        Comment = ownInformation.Comment,
                        CommentDate = DateTime.Now,
                        CommentUser = HPTConfig.Config.UserNameForUploads,
                        HasComment = true,
                        IsOwnComment = true,
                        NextTimer = ownInformation.NextTimer
                    };
                    ownInformation.HorseOwnInformationCommentList.Add(ownInformationComment);
                }
            }
            return ownInformation;
        }

        internal void CleanUpOldNextStarts()
        {
            this.HorseOwnInformationList
                .Where(oi => oi.NextStart != null)
                .Where(oi => oi.NextStart.StartDate < DateTime.Now.AddDays(-7D))
                .ToList()
                .ForEach(oi => this.HorseOwnInformationList.Remove(oi));
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string UserName { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Comment { get; set; }

        [DataMember]
        public DateTime LastUpdated { get; set; }
    }

    [DataContract]
    public class HPTHorseOwnInformation : Notifier
    {
        private int startNr;
        [DataMember]
        public int StartNr
        {
            get
            {
                return startNr;
            }
            set
            {
                startNr = value;
                OnPropertyChanged("StartNr");
            }
        }

        private string _ATGId;
        [DataMember]
        public string ATGId
        {
            get
            {
                return _ATGId;
            }
            set
            {
                this._ATGId = value;
                OnPropertyChanged("ATGId");
            }
        }
        private string name;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public string HorseNameWithoutInvalidCharacters
        {
            get
            {
                if (string.IsNullOrEmpty(this.Name))
                {
                    return "ERROR";
                }
                string correctedHorseName = this.Name;
                System.IO.Path.GetInvalidFileNameChars()
                    .ToList()
                    .ForEach(ic => correctedHorseName = correctedHorseName.Replace(ic, '_'));

                return correctedHorseName;
            }
        }

        private bool? nextTimer;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool? NextTimer
        {
            get
            {
                return nextTimer;
            }
            set
            {
                nextTimer = value;
                OnPropertyChanged("NextTimer");
                if (HorseOwnInformationCommentList != null && HorseOwnInformationCommentList.Count > 0)
                {
                    foreach (var comment in HorseOwnInformationCommentList)
                    {
                        comment.NextTimer = value;
                    }
                }
            }
        }

        private ObservableCollection<HPTHorseOwnInformationComment> horseOwnInformationCommentList;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTHorseOwnInformationComment> HorseOwnInformationCommentList
        {
            get
            {
                return horseOwnInformationCommentList;
            }
            set
            {
                horseOwnInformationCommentList = value;
                OnPropertyChanged("HorseOwnInformationCommentList");
            }
        }

        private string comment;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
                OnPropertyChanged("Comment");
                if (!string.IsNullOrEmpty(this.comment))
                {
                    this.HasComment = true;
                }
            }
        }

        private bool hasComment;
        [DataMember]
        public bool HasComment
        {
            get
            {
                return hasComment;
            }
            set
            {
                hasComment = value;
                OnPropertyChanged("HasComment");
            }
        }

        private HPTHorseNextStart _NextStart;
        [DataMember]
        public HPTHorseNextStart NextStart
        {
            get
            {
                return _NextStart;
            }
            set
            {
                this._NextStart = value;
                OnPropertyChanged("NextStart");
            }
        }


        private int _Age;
        [DataMember]
        public int Age
        {
            get
            {
                return _Age;
            }
            set
            {
                this._Age = value;
                OnPropertyChanged("Age");
            }
        }

        private string _Sex;
        [DataMember]
        public string Sex
        {
            get
            {
                return _Sex;
            }
            set
            {
                this._Sex = value;
                OnPropertyChanged("Sex");
            }
        }

        private string _Owner;
        [DataMember]
        public string Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                this._Owner = value;
                OnPropertyChanged("Owner");
            }
        }

        private string _Trainer;
        [DataMember]
        public string Trainer
        {
            get
            {
                return _Trainer;
            }
            set
            {
                this._Trainer = value;
                OnPropertyChanged("Trainer");
            }
        }

        private string _HomeTrack;
        [DataMember]
        public string HomeTrack
        {
            get
            {
                return _HomeTrack;
            }
            set
            {
                this._HomeTrack = value;
                OnPropertyChanged("HomeTrack");
            }
        }


        private DateTime _CreationDate;
        [DataMember]
        public DateTime CreationDate
        {
            get
            {
                return _CreationDate;
            }
            set
            {
                this._CreationDate = value;
                OnPropertyChanged("CreationDate");
            }
        }

        public string STLink   // För Saxade banor
        {
            get
            {
                return ATGLinkCreator.CreateSTHorseLink(this.ATGId);
            }
        }

        public DateTime LastUpdate   // För Saxade banor
        {
            get
            {
                if (this.HorseOwnInformationCommentList == null ||this.HorseOwnInformationCommentList.Count == 0)
                {
                    return this.CreationDate;
                }
                return this.HorseOwnInformationCommentList.Max(c => c.CommentDate);
            }
        }

        public bool Updated { get; set; }

        public static bool operator ==(HPTHorseOwnInformation oi1, HPTHorseOwnInformation oi2)
        {
            if ((object)oi1 == null && (object)oi2 == null)
            {
                return true;
            }
            if ((object)oi1 == null || (object)oi2 == null)
            {
                return false;
            }
            return oi1.Name == oi2.Name;
        }

        public static bool operator !=(HPTHorseOwnInformation oi1, HPTHorseOwnInformation oi2)
        {
            return !(oi1 == oi2);
        }
    }

    [DataContract]
    public class HPTHorseOwnInformationComment : Notifier
    {
        private string distance;
        [DataMember]
        public string Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = value;
                OnPropertyChanged("Distance");
            }
        }

        private string comment;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
                OnPropertyChanged("Comment");
                this.HasComment = !string.IsNullOrEmpty(this.comment);
            }
        }

        private DateTime commentDate;
        [DataMember]
        public DateTime CommentDate
        {
            get
            {
                return commentDate;
            }
            set
            {
                commentDate = value;
                OnPropertyChanged("CommentDate");
            }
        }

        private string commentUser;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string CommentUser
        {
            get
            {
                return commentUser;
            }
            set
            {
                commentUser = value;
                OnPropertyChanged("CommentUser");
            }
        }

        private bool hasComment;
        [DataMember]
        public bool HasComment
        {
            get
            {
                return hasComment;
            }
            set
            {
                hasComment = value;
                OnPropertyChanged("HasComment");
            }
        }

        private bool isOwnComment;
        [DataMember]
        public bool IsOwnComment
        {
            get
            {
                return isOwnComment;
            }
            set
            {
                isOwnComment = value;
                OnPropertyChanged("IsOwnComment");
            }
        }

        private bool? nextTimer;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool? NextTimer
        {
            get
            {
                return nextTimer;
            }
            set
            {
                nextTimer = value;
                OnPropertyChanged("NextTimer");
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(this.CommentDate.ToString("yyyy-MM-dd"));
            sb.AppendLine(this.Comment);
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
