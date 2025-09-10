using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    
    public enum BadPollTypeEnum
    {
        BinNotOnSensor = 1,
        WrongItemOnBin = 2,
        //eVMIComPortError = 3
    }

    public enum CalibrateStepEnum
    {
        Step1 = 1,
        Step2 = 2,
        Step3 = 3
    }

    public class PollEmailDTOCollection<T> where T : EVMIEmailItemDTO
    {
        public string EnterpriseDBName { get; set; }
        public long EnterpriseID { get; set; }

        public PollEmailDTOCollection(long enterpriseID, string enterpriseDBName)
        {
            this.EnterpriseDBName = enterpriseDBName;
            this.EnterpriseID = enterpriseID;
            this.Polls = new List<T>();
        }

        public PollEmailDTOCollection(long enterpriseID, string enterpriseDBName, List<T> polls)
        {
            this.EnterpriseDBName = enterpriseDBName;
            this.EnterpriseID = enterpriseID;
            this.Polls = polls;
        }

        public List<T> Polls { get; set; }

        public void Add(T obj)
        {
            this.Polls.Add(obj);
        }

        public int Count
        {
            get
            {
                return this.Polls.Count;
            }
        }

        public Dictionary<string, List<T>> GetGroupByRoom()
        {
            Dictionary<string, List<T>> grp = new Dictionary<string, List<T>>();
            foreach (var dto in this.Polls)
            {
                string key = dto.RoomID + "_" + dto.CompanyID;
                List<T> val;
                if (grp.ContainsKey(key))
                {
                    val = grp[key];
                    val.Add(dto);
                }
                else
                {
                    val = new List<T>();
                    val.Add(dto);
                    grp.Add(key, val);
                }
            }

            return grp;
        }

        public PollEmailDTOCollection<T> Where(Func<T, bool> predicate)
        {
            if (this.Polls != null && this.Polls.Count > 0)
            {
                List<T> filteredPolls = this.Polls.Where<T>(predicate).ToList();
                return new PollEmailDTOCollection<T>(this.EnterpriseID, this.EnterpriseDBName, filteredPolls);
            }
            return null;
        }

    }


    public class EVMIEmailDTO
    {
        public EVMIEmailDTO(long companyID, long roomID,
            string roomName, string companyName)
        {
            this.CompanyID = companyID;
            this.RoomID = roomID;
            this.RoomName = roomName;
            this.CompanyName = companyName;
        }

        public bool IsError
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ComError) && string.IsNullOrWhiteSpace(ExceptionError))
                {
                    return false;
                }
                return true;
            }
        }

        public string Status
        {
            get
            {
                string s = IsError ? "Fail" : "Success";
                return s;
            }
        }

        public string ComError { get; set; }
        public string WarningError { get; set; }

        public string ExceptionError { get; set; }

        public string ErrorDescription
        {
            get
            {
                int cnt = 1;
                string s = "";
                if (!string.IsNullOrWhiteSpace(ComError))
                {
                    s = cnt.ToString() + ". " + ComError;
                    cnt++;
                }
                if (!string.IsNullOrWhiteSpace(WarningError))
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        s += Environment.NewLine;
                    }
                    s += cnt.ToString() + ". Warning : " + WarningError;
                    cnt++;
                }
                if (!string.IsNullOrWhiteSpace(ExceptionError))
                {

                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        s += Environment.NewLine;
                    }
                    s += cnt.ToString() + ". Technical Error : " + ExceptionError;
                    cnt++;
                }

                return s;
            }
        }

        public long CompanyID { get; set; }

        public string CompanyName { get; set; }
        public long RoomID { get; set; }

        public string RoomName { get; set; }
    }

    public class EVMIEmailItemDTO : EVMIEmailDTO
    {

        public EVMIEmailItemDTO(long binID, int scaleId, int channelId,
            Guid itemGUID, long companyID, long roomID,
            string roomName, string companyName,
            string binNumber, string itemNumber
            ) : base(companyID, roomID, roomName, companyName)
        {
            this.BinID = binID;
            this.ScaleId = scaleId;
            this.ChannelId = channelId;

            this.ItemGUID = itemGUID;


            this.BinNumber = binNumber;
            this.ItemNumber = itemNumber;

        }



        public string ItemNumber { get; set; }

        public Guid ItemGUID { get; set; }

        public long BinID { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public string BinNumber { get; set; }

        public int ScaleId { get; set; }

        public int ChannelId { get; set; }

        

        public string SensorId
        {
            get
            {
                return ScaleId + "." + ChannelId;
            }
        }

    }

    public class BadPollEmailDTO : EVMIEmailItemDTO
    {
        public double WeightPerPiece { get; set; }
        public double WeightReading { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double WeightVariance { get; set; }

        public string BadPollErrorDetails
        {
            get
            {
                string error = "Bad Poll : ";
                foreach (var badPoll in this.BadPollType)
                {
                    error += badPoll.Value;
                }
                return error;
            }
        }

        public Dictionary<BadPollTypeEnum, string> BadPollType { get; set; }

        public double ItemCount
        {
            get
            {
                double cnt = 0;
                if (this.WeightPerPiece > 0)
                {
                    cnt = this.WeightReading / this.WeightPerPiece;
                }
                return Convert.ToDouble(cnt.ToString("#.###"));
            }
        }

        public BadPollEmailDTO(long binID, int scaleId, int channelId,
                Guid itemGUID, long companyID, long roomID,
                double weightPerPiece,
                string roomName, string companyName,
                string binNumber, string itemNumber,
                double weightVariance
                ) : base(binID, scaleId, channelId,
                 itemGUID, companyID, roomID,
                 roomName, companyName,
                 binNumber, itemNumber)
        {
            this.WeightPerPiece = weightPerPiece;
            this.WeightVariance = weightVariance;
            this.BadPollType = new Dictionary<BadPollTypeEnum, string>();
        }


        public bool Validate(out EVMIPollErrorTypeEnum pollErrorType)
        {
            bool b = true;
            pollErrorType = EVMIPollErrorTypeEnum.None;
            if (this.WeightReading < 0)
            {
                b = false;
                string error = "Bin not on sensor.";
                this.BadPollType.Add(BadPollTypeEnum.BinNotOnSensor, error);
                pollErrorType = EVMIPollErrorTypeEnum.DataError;
            }
            else if (this.WeightPerPiece <= 0)
            {
                b = false;
                string error = "Weight Per Piece must be greater than Zero.";
                this.BadPollType.Add(BadPollTypeEnum.WrongItemOnBin, error);
                pollErrorType = EVMIPollErrorTypeEnum.DataError;
            }
            else if (this.WeightReading > 0 && this.WeightPerPiece > this.WeightReading)
            {
                double itemCount = this.ItemCount; //this.SensorWeight / this.WeightPerPiece;
                bool isWholeNumber = Math.Abs(itemCount % 1) <= (Double.Epsilon * 100);

                if (!isWholeNumber)
                {
                    b = false;
                    string error = "Item Count " + itemCount + " is not a whole number.";
                    this.BadPollType.Add(BadPollTypeEnum.WrongItemOnBin, error);
                    pollErrorType = EVMIPollErrorTypeEnum.Warning;
                }
            }
            else if (this.WeightReading > 0 && this.WeightPerPiece < this.WeightReading)
            {
                double itemCount = this.ItemCount; //this.SensorWeight / this.WeightPerPiece;
                bool isWholeNumber = Math.Abs(itemCount % 1) <= (Double.Epsilon * 100);

                if (!isWholeNumber)
                {
                    b = false;
                    string error = "Item Count " + itemCount + " is not a whole number.";
                    this.BadPollType.Add(BadPollTypeEnum.WrongItemOnBin, error);
                    pollErrorType = EVMIPollErrorTypeEnum.Warning;
                }
            }

            return b;
        }

    }

    public class PollDoneEmailDTO : EVMIEmailItemDTO
    {
        public string PollAction { get; set; }

        public double NewQuantity { get; set; }

        public double PoolQuantity { get; set; }

        public double WeightPerPiece { get; set; }
        public double WeightReading { get; set; }
        public PollDoneEmailDTO(long binID, int scaleId, int channelId, double sensorWeight,
                    Guid itemGUID, long companyID, long roomID,

                    string roomName, string companyName,
                    string binNumber, string itemNumber
                    ) : base(binID, scaleId, channelId,
                     itemGUID, companyID, roomID,
                     roomName, companyName,
                     binNumber, itemNumber)
        {

            this.WeightReading = sensorWeight;
        }


    }

    public class TareDoneEmailDTO : EVMIEmailItemDTO
    {

        public TareDoneEmailDTO(long binID, int scaleId, int channelId,
                   Guid itemGUID, long companyID, long roomID,
                   string roomName, string companyName,
                   string binNumber, string itemNumber
                   ) : base(binID, scaleId, channelId,
                    itemGUID, companyID, roomID,
                    roomName, companyName,
                    binNumber, itemNumber)
        {

        }
    }

    public class ResetDoneEmailDTO : EVMIEmailItemDTO
    {

        public ResetDoneEmailDTO(long binID, int scaleId, int channelId,
                   Guid itemGUID, long companyID, long roomID,
                   string roomName, string companyName,
                   string binNumber, string itemNumber
                   ) : base(binID, scaleId, channelId,
                    itemGUID, companyID, roomID,
                    roomName, companyName,
                    binNumber, itemNumber)
        {

        }
    }

    public class CommonRequestDoneEmailDTO : EVMIEmailItemDTO
    {

        public CommonRequestDoneEmailDTO(long binID, int scaleId, int channelId,
                   Guid itemGUID, long companyID, long roomID,
                   string roomName, string companyName,
                   string binNumber, string itemNumber
                   ) : base(binID, scaleId, channelId,
                    itemGUID, companyID, roomID,
                    roomName, companyName,
                    binNumber, itemNumber)
        {

        }
    }

    public class GetCalibrationWeightDoneEmailDTO : EVMIEmailItemDTO
    {

        public GetCalibrationWeightDoneEmailDTO(long binID, int scaleId, int channelId,
                   Guid itemGUID, long companyID, long roomID,
                   string roomName, string companyName,
                   string binNumber, string itemNumber
                   ) : base(binID, scaleId, channelId,
                    itemGUID, companyID, roomID,
                    roomName, companyName,
                    binNumber, itemNumber)
        {

        }
    }

    public class SetCalibrationWeightDoneEmailDTO : EVMIEmailItemDTO
    {

        public SetCalibrationWeightDoneEmailDTO(long binID, int scaleId, int channelId,
                   Guid itemGUID, long companyID, long roomID,
                   string roomName, string companyName,
                   string binNumber, string itemNumber
                   ) : base(binID, scaleId, channelId,
                    itemGUID, companyID, roomID,
                    roomName, companyName,
                    binNumber, itemNumber)
        {

        }
    }


    public class CalibrateDoneEmail : EVMIEmailItemDTO
    {

        public CalibrateStepEnum StepNo { get; set; }

        public CalibrateDoneEmail(long binID, int scaleId, int channelId,
                       Guid itemGUID, long companyID, long roomID,
                       string roomName, string companyName,
                       string binNumber, string itemNumber,
                       CalibrateStepEnum stepNo
                       ) : base(binID, scaleId, channelId,
                        itemGUID, companyID, roomID,
                        roomName, companyName,
                        binNumber, itemNumber)
        {
            this.StepNo = stepNo;
        }
    }

    public class ItemWeightPerPieceDoneEmailDTO : EVMIEmailItemDTO
    {

        public double WeightReading { get; set; }
        public ItemWeightPerPieceDoneEmailDTO(long binID, int scaleId, int channelId,
                   Guid itemGUID, long companyID, long roomID,
                   string roomName, string companyName,
                   string binNumber, string itemNumber
                   ) : base(binID, scaleId, channelId,
                    itemGUID, companyID, roomID,
                    roomName, companyName,
                    binNumber, itemNumber)
        {

        }
    }

    /// <summary>
    /// Get or Set shelf id ( scale id )
    /// </summary>
    public class ShelfIDEmailDTO : EVMIEmailDTO
    {
        public string EnterpriseDBName { get; set; }
        public long EnterpriseID { get; set; }

        public int ScaleID { get; set; }

        public eVMIShelfRequestType ShelfRequestType { get; set; }

        public ShelfIDEmailDTO(string enterpriseDBName, long enterpriseID, long companyID, long roomID,
            string roomName, string companyName) : base(companyID, roomID,
             roomName, companyName)
        {
            this.EnterpriseDBName = enterpriseDBName;
            this.EnterpriseID = enterpriseID;
        }
    }


}
