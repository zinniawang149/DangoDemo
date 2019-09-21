import { Injectable } from "@angular/core";
import { Resolve, ActivatedRouteSnapshot, Router } from "@angular/router";
import { User } from "../_models/user";
import { Observable, of } from "rxjs";
import { UserService } from "../_services/user.service";
import { AlertifyService } from "../_services/alertify.service";
import { catchError } from "rxjs/operators";
import { PaginatedResult } from "../_models/pagination";
import { Photo } from "../_models/photo";
import { AdminService } from "../_services/admin.service";

@Injectable()
export class PhotoManagementResolver implements Resolve<Photo[]>{

  constructor(private adminService: AdminService, private router: Router, private alertify: AlertifyService) { }
  resolve(route: ActivatedRouteSnapshot): Observable<Photo[]> {
    return this.adminService.getPhotosForModeration().pipe(
      catchError(error => {
      this.alertify.error('Problem retrieving data');
      this.router.navigate(['/home']);
      return of(null);
    }));
    }

}
