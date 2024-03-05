import {
  Injectable,
  Injector,
  ComponentFactoryResolver,
  EmbeddedViewRef,
  ApplicationRef
} from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class DomService {

  private childComponentRef: any[] = [];
  constructor(
    private componentFactoryResolver: ComponentFactoryResolver,
    private appRef: ApplicationRef,
    private injector: Injector
  ) { }

  public appendComponentTo(parentId: string, child: any, childConfig?: childConfig) {
    // Create a component reference from the component
    const childComponentRef = this.componentFactoryResolver
      .resolveComponentFactory(child)
      .create(this.injector);

    // Attach the config to the child (inputs and outputs)
    this.attachConfig(childConfig, childComponentRef);

    this.childComponentRef.push(childComponentRef);
    // Attach component to the appRef so that it's inside the ng component tree
    this.appRef.attachView(childComponentRef.hostView);

    // Get DOM element from component
    const childDomElem = (childComponentRef.hostView as EmbeddedViewRef<any>)
      .rootNodes[0] as HTMLElement;

    // Append DOM element to the body
    document.getElementById(parentId).appendChild(childDomElem);
    // console.log('on append---', this.childComponentRef);
  }

  public removeComponent() {
    this.appRef.detachView(this.childComponentRef[this.childComponentRef.length - 1].hostView);
    this.childComponentRef[this.childComponentRef.length - 1].destroy();
    this.childComponentRef.splice(-1, 1);

    if (this.childComponentRef.length < 1) {
      document.getElementById('modal-container').className = 'hidden';
      document.getElementById('overlay').className = 'hidden';
    }

    // console.log('on remove---', this.childComponentRef);
  }


  private attachConfig(config, componentRef) {
    const inputs = config.inputs;
    const outputs = config.outputs;

    componentRef.instance['inputs'] = inputs;
    componentRef.instance['outputs'] = outputs;
  }
}

interface childConfig {
  inputs: object;
  outputs: object;
}
