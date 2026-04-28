import {BackendService} from '../services/backend-service';
import {inject} from 'aurelia-framework';
import {TakeoffForm} from "../model/takeoff-form";

@inject(BackendService)
export class CurrentFlights {

  constructor(backendService) {
    this.backendService = backendService;

    this.airplanes = [];
    this.clubMembers = [];

    this.takeoffForm = new TakeoffForm();
  }

  activate(params) {
    this.backendService.getClubAirplanes().then(
      data => {
        this.airplanes = data;
        this.applyDropdownDefaults();
      }
    );

    this.backendService.getClubMembers().then(
      data => {
        this.clubMembers = data;
        this.applyDropdownDefaults();
      }
    );
  }

  applyDropdownDefaults() {
    if (this.airplanes.length) {
      this.takeoffForm.towplane.airplane.clubAirplane = this.airplanes[0];
      this.takeoffForm.glider.airplane.clubAirplane = this.airplanes[0];
    }
    if (this.clubMembers.length) {
      this.takeoffForm.towplane.pilot.clubMember = this.clubMembers[0];
      this.takeoffForm.glider.pilot.clubMember = this.clubMembers[0];
    }
  }

  takeoff() {
    this.backendService.takeoff(this.takeoffForm)
      .then(() => {
        alert('Start letu byl zaznamenán');
        this.takeoffForm = new TakeoffForm();
        this.applyDropdownDefaults();
      })
      .catch(error => {
        console.log(error);
        alert('Start letu se nepodařilo zaznamenat');
      });
  }

}
