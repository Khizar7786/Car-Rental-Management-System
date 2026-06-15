import { RouterModule,Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { HomeComponent } from './home-component/home-component';
import { LoginComponent } from './login/login';
import { Vehichle } from './vehichle/vehichle';
import { Invoice } from './invoice/invoice';
import { Customer } from './customer/customer';
import { SignupComponent } from './signup/signup';
import { Agreement } from './agreement/agreement';

export const routes: Routes = [

  { path: '', component: HomeComponent },

  { path: 'vehicle', component: Vehichle },
  { path: 'invoice', component: Invoice },
  { path: 'customer', component: Customer },
  {path:'agreement', component: Agreement},
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent }

];
@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      onSameUrlNavigation: 'reload'
    })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {}