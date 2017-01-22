using System;
using UnityEngine;

public class Action1000 : GameAction
{
    public class Passport
    {
        public string PassportId { get; set; }
        public string Password { get; set; }
    }
    private ActionResult actionResult;

    public Action1000()
        : base((int)ActionType.CreateRoom)
    {
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {
        //TODO:登录服务器获取账号
        //writer.writeString("Handler", "Passport");
        //writer.writeString("IMEI", GameSetting.Instance.DeviceID);

        
    }

    protected override void DecodePackage(NetReader reader)
    {
        actionResult = new ActionResult();
        actionResult["passportID"] = reader.readString();
        actionResult["password"] = reader.readString();

        //TODO:登录服务器获取账号
        //var passport = reader.readValue<Passport>();
        //actionResult["passportID"] = passport.PassportId;
        //actionResult["password"] = passport.Password;

    }

    public override ActionResult GetResponseData()
    {
        return actionResult;
    }
}
