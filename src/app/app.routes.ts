import { Routes } from '@angular/router';
import { HomeComponent } from './home-component/home-component';
import { LoginComponent } from './login/login';
import { Vehichle } from './vehichle/vehichle';
import { Agreement } from './agreement/agreement';
import { Invoice } from './invoice/invoice';
import { Customer } from './customer/customer';


export const routes: Routes = [
  { path: '', component: HomeComponent},
  { path: 'login', component: LoginComponent },
  {path:'vehicle', component: Vehichle},
  {path:'agreement', component:Agreement},
  {path: 'invoice', component:Invoice},
  {path:'customer', component:Customer}
];