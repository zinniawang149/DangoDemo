import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { AlertifyService } from '../../_services/alertify.service';

@Component({
  selector: 'app-photo-card',
  templateUrl: './photo-card.component.html',
  styleUrls: ['./photo-card.component.css']
})

export class PhotoCardComponent implements OnInit {
  @Input() photo;
  @Output() photoChange = new EventEmitter<PhotoChange>();
  constructor(private adminService: AdminService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  setApprovedPhoto(photoId: number) {
    this.alertify.confirm('Are you sure to approve this photo?', () => {
      this.adminService.setApprovedPhoto(photoId).subscribe(() => {
        this.photoChange.emit(photoId);
      }, error => {
        this.alertify.error(error);
      });
    })
  }

  rejectPhoto(photoId: number) {
    this.alertify.confirm('Are you sure to reject this photo?', () => {
      this.adminService.rejectPhoto(photoId).subscribe(() => {
        this.photoChange.emit(photoId);
      }, error => {
        this.alertify.error(error);
      });
    })
  }
}
