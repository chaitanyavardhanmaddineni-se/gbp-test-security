import { Injectable } from "@angular/core";
import {
  Resolve,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from "@angular/router";
import { ApiService } from "./api.service";
import { ApiDictionary } from './api-dictionary';
import { BillpaydataService } from './billpaydata.service'

import {
  CbmsBaseUtilService,
  PermissionService,
  CbmsService
} from "se-cbms-base";
import { Location } from "@angular/common";
import { environment } from '../../../environments/environment';
import { UserobjModel } from '../../core/models/model';



@Injectable({
  providedIn: 'root'
})
export class UserResolver implements Resolve<any> {
  userInfoObj: any;
  userDataObj: any;
  public userInfoId: any;
  uid: any;
  public AuthObj;
  //public userObj:any;
  private invoiceId: number;
  private userObjModel: UserobjModel = new UserobjModel();

  constructor(
    private apiService: ApiService,
    private cbmsService: CbmsService,
    private basePermission: PermissionService,
    private cbmsUtil: CbmsBaseUtilService,
    private location: Location,
    private userDataservice: BillpaydataService
  ) {
    if (this.location.path() !== '' && this.location.path().indexOf('uid') !== -1) {
      this.AuthObj = {
        id: this.location
          .path()
          .split('?')[1]
          .split('uid=')[1]
          .split('&')[0],
        type: 'uid'
      };
    }
    if (this.location.path() !== '' && this.location.path().indexOf('xid') !== -1) {
      this.AuthObj = {
        id: this.location
          .path()
          .split('?')[1]
          .split('xid=')[1]
          .split('&')[0],
        type: 'xid'
      };
    }
    else if (this.location.path() !== '' && this.location.path().indexOf('ViewClientInfo') !== -1) {
      this.AuthObj = {
        id: this.location.path().split('/')[this.location.path().split('/').length - 1],
        type: 'uid'
      }
    }
  }
  async resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (this.AuthObj !== undefined) {
      if (this.AuthObj.type === 'uid' && this.AuthObj.id != "") {
        this.userInfoObj = await this.cbmsUtil.authenticateUser(this.AuthObj);
        this.userInfoId = await this.cbmsUtil.UidValidation(this.AuthObj);
        this.userDataObj = await this.GetUserDetailsAsync(this.userInfoId);
        const uid = decodeURIComponent(
          (this.AuthObj.id + '').replace(/\+/g, '%3D')
        );
        this.buildUserDataInfo(uid, "uid")
      } else if (this.AuthObj.type === 'xid' && this.AuthObj.id != "") {
        const objUserDetails: any = {};
        const xid = decodeURIComponent(
          (this.AuthObj.id + '').replace(/\+/g, '%3D')
        );
        // let xidValidate =await this.cbmsService.Validatexid(xid);
        this.userDataObj = await this.GetUserDetailsByXid(xid);
        if (!this.userDataObj)
          window.location.assign(environment.appLoginURL + "/cbms/index.jsp");

        this.buildUserDataInfo(xid, "xId")
      }
    }
    else {
      window.location.assign(environment.appLoginURL + "/cbms/index.jsp");
    }
  }
  private buildUserDataInfo(id, type) {
    this.userObjModel.userInfoId = this.userDataObj.userInfoId;
    this.userObjModel.userQueueId = this.userDataObj.userQueueId;
    this.userObjModel.userName = this.userDataObj.userName;
    this.userObjModel.xId = type == "xId" ? id : "";
    this.userObjModel.uid = type == "uid" ? id : "";
    // this.userObj = {
    //   userInfoId: this.userDataObj.userInfoId,
    //   userQueueId: this.userDataObj.userQueueId ,
    //   userName: this.userDataObj.userName,  
    //   xId: type == "xId"?id:"",
    //   uid: type == "uid"?id:"",
    //   invoiceId: this.invoiceId
    // };
    this.userDataservice.setuserInfo(this.userObjModel);
  }
  async GetUserDetailsByXid(xid) {
    return await this.apiService.getasync(ApiDictionary.GetUserDetailsByXid.url + '?xid=' + xid);
  }
  async GetUserDetailsAsync(userInfoId) {
    return await this.apiService.getasync(ApiDictionary.GetUserDetailsByID.url + '?userInfoid=' + userInfoId);
  }
}
