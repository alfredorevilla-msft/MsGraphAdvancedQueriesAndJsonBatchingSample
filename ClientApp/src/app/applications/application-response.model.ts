import { ApplicationModel } from "./application.model";

export interface ApplicationResponseModel {
  list: ApplicationModel[];
  total: number;
}
