import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  model: any = {}


  //inject account service
  constructor(public accountService: AccountService, private router: Router,
     private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  login() {
    this.accountService.loginuser(this.model).subscribe(response => {
     this.router.navigateByUrl('/members')
      
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/')
 
  }

}
