<?php
// Copyright (C) 2020-2022 FoxxiBot
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

// Check for Secure Connection
if (!defined("G_FW") or !constant("G_FW")) die("Direct access not allowed!");

$options = array();

$result = $PDO->query("SELECT * FROM gb_twitch_options");
foreach($result as $row)
{
  $options[$row["parameter"]] = $row["value"];
}
?>

  <form method="post" enctype="multipart/form-data" action="<?php print $gfw["site_url"]; ?>/index.php?p=giveaway&a=funcs&v=save">

  <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <div class="container-fluid">
        <div class="row mb-2">
          <div class="col-sm-6">
            <h1>Giveaway Management</h1>
          </div>
          <div class="col-sm-6">
            <ol class="breadcrumb float-sm-right">
              <li class="breadcrumb-item"><a href="#">Home</a></li>
              <li class="breadcrumb-item active">Giveaway Management</li>
            </ol>
          </div>
        </div>
      </div><!-- /.container-fluid -->
    </section>

    <!-- Main content -->
    <section class="content">

    <div class="container-fluid">
        <div class="row">
          <div class="col-12">

            <div class="card">
              <div class="card-header">
                <h3 class="card-title">Here is Giveaway Information.</h3>

                <div class="card-tools">
                  <h3 style="margin-top: 4px;" class="card-title">Status:&nbsp;&nbsp;</h3>
                  <?php
                  if (!isset($options["Giveaway_Status"]) || $options["Giveaway_Status"] == "off") {
                    print '<input type="checkbox" name="Giveaway_Status" data-bootstrap-switch data-off-color="danger" data-on-color="success">';
                  } else {
                    print '<input type="checkbox" name="Giveaway_Status" checked data-bootstrap-switch data-off-color="danger" data-on-color="success">';
                  }
                ?>
                </div>                
              </div>
              <!-- /.card-header -->
            
              <div class="card-body">

              <div class="row">
                  
                  <div class="col-md-4">
                    <div class="card">
                      <div class="card-header">
                        <h3 style="margin-top: 3px;" class="card-title">Giveaway Details</h3>
                      </div>
                    
                      <div class="card-body">

                        <div class="form-group">
                          <label for="giveawayName">Giveaway Name</label>

                          <?php
                          if (isset($options["Giveaway_Name"])) {
                            print "<input type=\"text\" class=\"form-control\" id=\"giveawayName\" name=\"giveawayName\" placeholder=\"Enter Giveaway Name\" value=\"$options[Giveaway_Name]\" required>";
                          } else {
                            print "<input type=\"text\" class=\"form-control\" id=\"giveawayName\" name=\"giveawayName\" placeholder=\"Enter Giveaway Name\" required>";
                          }
                          ?>
                        </div>
                      
                        <div class="form-group">
                          <label for="giveawayDetails">Giveaway Details</label>
                          <textarea class="form-control" rows="3"  id="giveawayDetails" name="giveawayDetails" placeholder="Ex. Want to win a bla, then type !giveaway to be added to the draw" required><?php if (isset($options["Giveaway_Details"])) { print $options["Giveaway_Details"]; } ?></textarea>
                        </div>

                      </div>
                      <!-- /.card-body -->

                      <div class="card-footer">
                        <input type="hidden" id="submit" name="submit" value="submit">
                        <a onclick="return confirm('Are you sure you want to clear all data, including giveaway participants?');" href="<?php print $gfw["site_url"]; ?>/index.php?p=giveaway&a=funcs&v=clear" class="btn btn-danger">Clear all Data</a>
                        <button style="float: right;" type="submit" class="btn btn-primary">Save Data</button>
                      </div>

                    </div>
                    <!-- /.card -->
                  </div>

                  <div class="col-md-4">
                    <div class="card">
                      <div class="card-header">
                        <h3 style="margin-top: 3px;" class="card-title">Giveaway Options</h3>
                      </div>
                    
                      <div class="card-body">

                        <div class="form-group">
                          <label>Allow Twitch Staff to Join Giveaway?</label>
                          <div style="float: right;">
                          <?php
                          if (!isset($options["Giveaway_AllowTwitchStaff"]) || $options["Giveaway_AllowTwitchStaff"] == "off") {
                            print '<input type="checkbox" name="Giveaway_AllowTwitchStaff" data-bootstrap-switch data-off-color="danger" data-on-color="success">';
                          } else {
                            print '<input type="checkbox" name="Giveaway_AllowTwitchStaff" checked data-bootstrap-switch data-off-color="danger" data-on-color="success">';
                          }
                          ?>
                          </div>
                        </div>

                        <div class="form-group">
                          <label>Allow Mods to Join Giveaway?</label>
                          <div style="float: right;">
                          <?php
                          if (!isset($options["Giveaway_AllowMods"]) || $options["Giveaway_AllowMods"] == "off") {
                            print '<input type="checkbox" name="Giveaway_AllowMods" data-bootstrap-switch data-off-color="danger" data-on-color="success">';
                          } else {
                            print '<input type="checkbox" name="Giveaway_AllowMods" checked data-bootstrap-switch data-off-color="danger" data-on-color="success">';
                          }
                          ?>
                          </div>
                        </div>

                      </div>
                      <!-- /.card-body -->

                      <div class="card-footer">
                        <input type="hidden" id="submit" name="submit" value="submit">
                        <button style="float: right;" type="submit" class="btn btn-primary">Save Data</button>
                      </div>

                    </div>
                    <!-- /.card -->
                  </div>

                  <div class="col-md-4">
                    <div class="card">
                      <div class="card-header">
                        <h3 style="margin-top: 3px;" class="card-title">Giveaway Results</h3>

                        <div class="card-tools">
                          <?php
                          $gw_statement = "SELECT COUNT(*) FROM gb_twitch_giveaway"; 
                          $gw_result = $PDO->prepare($gw_statement); 
                          $gw_result->execute(); 
                          $number_of_rows = $gw_result->fetchColumn(); 
                          ?>
                          <h3 style="margin-top: 3px;" class="card-title"><?php echo $number_of_rows; ?> Participants</h3>
                        </div>
                      </div>
                    
                      <div class="card-body">

                      <ul class="nav flex-column">

                      <?php
                        $n_result = $PDO->query("SELECT value FROM gb_twitch_options WHERE parameter='Giveaway_Winner' LIMIT 1");
                        foreach($n_result as $n_row)
                        {
                          $winner = json_decode($n_row["value"], true);

                          if (!empty($winner)) {
                          print '
                          <li class="nav-item">
                            <span class="nav-link">User ID:
                              <span class="float-right">'. $winner->data[0]->uid .'</span>
                            </span>
                          </li>

                          <li class="nav-item">
                            <span class="nav-link">Display Name:
                              <span class="float-right">'. $winner->data[0]->display_name .'</span>
                            </span>
                          </li>

                          <li class="nav-item">
                            <span class="nav-link">Username:
                              <span class="float-right">'. $winner->data[0]->username .'</span>
                            </span>
                          </li>
                          ';
                          } else {
                            print '
                            <li class="nav-item">
                            <span class="nav-link">System:
                              <span class="float-right">No giveaway winner yet</span>
                            </span>
                          </li>
                          ';
                          }
                        }
                      ?>

                      </ul>

                      </div>
                      <!-- /.card-body -->
                    </div>
                    <!-- /.card -->
                  </div>

              </div>

              </div>
              <!-- /.card-body -->
            
            </div>
            <!-- /.card -->
          </div>
          <!-- /.col -->
        </div>
        <!-- /.row -->
      </div>
      <!-- /.container-fluid -->
 
    </section>
    <!-- /.content -->

  </div>
  <!-- /.content-wrapper -->

  </form>