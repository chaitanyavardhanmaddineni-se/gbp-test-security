import { UrlTree, DefaultUrlSerializer, UrlSerializer } from "@angular/router";
export class cleanUrlSerializer extends DefaultUrlSerializer {
  private _defaultUrlSerializer: DefaultUrlSerializer = new DefaultUrlSerializer();
  xid: string = "";
  uid: string = "";
  parse(url: string): UrlTree {
    // Encode "+" to "%2B"
    var stringurl;
    if(this.xid !=""||this.uid!=""){
      let id = this.xid!=""?"xid="+this.xid:"uid="+this.uid
      url = url+"?"+id
    }
    if (url != "" && url.indexOf("xid") !== -1) {
      var data = url
      this.xid = data
        .split('?')[1]
        .split('xid=')[1]
        .split('&')[0],
        // this.xid = decodeURIComponent(data.replace(/\+/g, "%3D"));
       stringurl = url.split("?")[0];
    }
    if (url != "" && url.indexOf("uid") !== -1) {
      var data = url
      this.uid = data
        .split('?')[1]
        .split('uid=')[1]
        .split('&')[0],
        //this.uid = decodeURIComponent(data.replace(/\+/g, "%3D"));
        stringurl = url.split("?")[0];
    }
    //url = url.replace(/\+/gi, "%2B");
    // Use the default serializer.
    if(stringurl!=undefined){
    return this._defaultUrlSerializer.parse(stringurl);
    }
  }

  serialize(tree: UrlTree): string {
    if (this.uid != "") {
      this.uid.replace("=", "%3D")
      return (
        this._defaultUrlSerializer.serialize(tree) +
        "?uid=" + this.uid
      );
    } else if (this.xid != "") {
      this.xid.replace("=", "%3D")
      return (
        this._defaultUrlSerializer.serialize(tree) +
        "?xid=" + this.xid
      );
    } else {
      return this._defaultUrlSerializer.serialize(tree);
    }
   this.xid = "";
    this.xid= "";
  }
}
