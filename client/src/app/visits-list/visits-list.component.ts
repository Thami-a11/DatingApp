import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagination';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-visits-list',
  templateUrl: './visits-list.component.html',
  styleUrls: ['./visits-list.component.css']
})
export class VisitsListComponent implements OnInit {
  members: Partial<Member[]>;
  predicate = 'visited'; 
  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination;
  title: string;

  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    this.loadVisits();
  }

  loadVisits(){
    this.memberService.getVisits(this.predicate,this.pageNumber,this.pageSize).subscribe(response=>{
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }

  pageChanged(event: any){
    this.pageNumber = event.page;
     this.loadVisits();
  }

  
 

}
