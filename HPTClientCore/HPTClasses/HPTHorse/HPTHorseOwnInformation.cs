using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseOwnInformationCollection : Notifier
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTHorseOwnInformation> HorseOwnInformationList { get; set; }

        internal void SaveHorseOwnInformationList()
        {
            var fileName = Path.Combine(HPTConfig.MyDocumentsPath, "HorseOwnInformationList.hptinfo");
            HPTSerializer.SerializeHPTHorseOwnInformation(fileName, this);
        }

        internal HPTHorseOwnInformation MergeHorseOwnInformation(HPTHorse horse)
        {
            try
            {
                if (HorseOwnInformationList == null)
                {
                    HorseOwnInformationList = new ObservableCollection<HPTHorseOwnInformation>();
                }
                var savedHorseInformation = HorseOwnInformationList.FirstOrDefault(oi => oi.Name == horse.HorseName);
                if (horse.OwnInformation == savedHorseInformation)
                {
                    return horse.OwnInformation;
                }
                if (savedHorseInformation == null && horse.OwnInformation != null)
                {
                    HorseOwnInformationList.Add(horse.OwnInformation);
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

                // Spara filen efter varje ändring (fire-and-forget med Task)
                _ = Task.Run(() => SaveHorseOwnInformationList());

                return savedHorseInformation;
            }
            catch (Exception)
            {
                return horse.OwnInformation;
            }
        }

        internal HPTHorseOwnInformation GetOwnInformationByName(string name)
        {
            if (HorseOwnInformationList == null)
            {
                HorseOwnInformationList = new ObservableCollection<HPTHorseOwnInformation>();
            }

            var ownInformation = HorseOwnInformationList.FirstOrDefault(oi => oi != null && oi.Name != null && oi.Name == name);
            if (ownInformation != null)
            {
                if (string.IsNullOrEmpty(ownInformation.Comment)
                    && (ownInformation.HorseOwnInformationCommentList == null || ownInformation.HorseOwnInformationCommentList.Count == 0)
                    && (ownInformation.NextTimer == null || ownInformation.NextTimer == false))
                {
                    try
                    {
                        HorseOwnInformationList.Remove(ownInformation);
                    }
                    catch (Exception exc)
                    {
                        var s = exc.Message;
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
            HorseOwnInformationList
                .Where(oi => oi.NextStart != null)
                .Where(oi => oi.NextStart.StartDate < DateTime.Now.AddDays(-7D))
                .ToList()
                .ForEach(oi => HorseOwnInformationList.Remove(oi));
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
        [DataMember]
        public int StartNr
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string ATGId
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Name
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public string HorseNameWithoutInvalidCharacters
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                {
                    return "ERROR";
                }
                var correctedHorseName = Name;
                System.IO.Path.GetInvalidFileNameChars()
                    .ToList()
                    .ForEach(ic => correctedHorseName = correctedHorseName.Replace(ic, '_'));

                return correctedHorseName;
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool? NextTimer
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
                if (HorseOwnInformationCommentList != null && HorseOwnInformationCommentList.Count > 0)
                {
                    foreach (var comment in HorseOwnInformationCommentList)
                    {
                        comment.NextTimer = value;
                    }
                }
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTHorseOwnInformationComment> HorseOwnInformationCommentList
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Comment
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(field))
                {
                    HasComment = true;
                }
            }
        }

        [DataMember]
        public bool HasComment
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public HPTHorseNextStart NextStart
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }


        [DataMember]
        public int Age
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string Sex
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string Owner
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string Trainer
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string HomeTrack
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }


        [DataMember]
        public DateTime CreationDate
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public string STLink
        {
            get
            {
                return ATGLinkCreator.CreateSTHorseLink(ATGId);
            }
        }

        public DateTime LastUpdate
        {
            get
            {
                if (HorseOwnInformationCommentList == null || HorseOwnInformationCommentList.Count == 0)
                {
                    return CreationDate;
                }
                return HorseOwnInformationCommentList.Max(c => c.CommentDate);
            }
        }

        public bool Updated { get; set; }

        public static bool operator ==(HPTHorseOwnInformation? oi1, HPTHorseOwnInformation? oi2)
        {
            return oi1 is not null && oi2 is not null
                && oi1.Name == oi2.Name;
        }

        public static bool operator !=(HPTHorseOwnInformation? oi1, HPTHorseOwnInformation? oi2)
        {
            return !(oi1 == oi2);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as HPTHorseOwnInformation);
        }

        public bool Equals(HPTHorseOwnInformation? other)
        {
            return other is not null && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }
    }

    [DataContract]
    public class HPTHorseOwnInformationComment : Notifier
    {
        [DataMember]
        public string Distance
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Comment
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
                HasComment = !string.IsNullOrEmpty(field);
            }
        }

        [DataMember]
        public DateTime CommentDate
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string CommentUser
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool HasComment
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool IsOwnComment
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool? NextTimer
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(CommentDate.ToString("yyyy-MM-dd"));
            sb.AppendLine(Comment);
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
