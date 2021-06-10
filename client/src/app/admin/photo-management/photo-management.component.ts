import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { PhotoEditorComponent } from 'src/app/members/photo-editor/photo-editor.component';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/Photo';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
User: User;
Photo: Photo[]=[];
member: Member;
username:string;
  
  constructor(private adminService: AdminService, private toastr:ToastrService) { }

  ngOnInit(): void {
    this.getPhotoForApproval();
  }

  getPhotoForApproval()
  {
    this.adminService.getPhotoForApproval().subscribe(Photo=>{
      this.Photo = Photo;
    });
  }
  approvePhoto(photoId: number){
    this.adminService.approvePhoto(photoId).subscribe(()=>{
      this.toastr.success("Approved");
      this.getPhotoForApproval();
    });
  }

  rejectPhoto(photoId: number){
    this.adminService.rejectPhoto(photoId).subscribe(()=>{
      this.toastr.success("Rejected");
      this.getPhotoForApproval();
    });
  }
}
