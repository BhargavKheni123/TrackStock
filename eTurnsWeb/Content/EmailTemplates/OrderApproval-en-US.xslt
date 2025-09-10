<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet exclude-result-prefixes="msxsl" version="1.0" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="eturnslogourl"></xsl:param>
  <xsl:param name="copyrightyear"></xsl:param>
  <xsl:template match="/">
    <table style="width: 8in;">
      <thead>
        <tr>
          <th style="text-align: left;">
            <a href="#" title="E Turns Powered">
              <img alt="E Turns Powered" src="{$eturnslogourl}" style="border: 0;" />
            </a>
          </th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td height="15" style="width:100%;">
          </td>
        </tr>
        <tr>
          <td>
            <table border="0" cellpadding="0" cellspacing="0" height="258" style="background: none repeat scroll 0% 0% rgb(240, 240, 240); border: 1px solid rgb(214, 214, 214);" width="751">
              <thead>
                <tr>
                  <td>
                    <span style="color: rgb(0, 0, 255);">
                      <span style="font-size: 24px;">
                        <strong>Hello,</strong>
                      </span>
                    </span>
                  </td>
                </tr>
                <tr>
                  <td style="text-align: left; vertical-align: top; width: 100%; height: 25px;">
                  </td>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>
                    A new order is submitted to the system and needs your approval to proceed further. Following are the details of the order for your ready reference:
                  </td>
                </tr>
                <tr>
                  <td style="text-align: left; width: 99%; height: 25px;">
                  </td>
                </tr>
                <tr>
                  <td style="width: 99%;">
                    <table align="left" border="0" cellpadding="0" cellspacing="0" height="84" style="height: 25px;" width="740">
                      <tbody>
                        <tr>
                          <td style="width: 49%; text-align: left; vertical-align: top; height: 25px;">
                            <strong>
                              Order#: <xsl:value-of select="OrderMasterDTO/OrderNumber"></xsl:value-of>
                            </strong>
                          </td>
                          <td style="width: 49%;">
                            Required Date: <xsl:value-of select="OrderMasterDTO/RequiredDate"></xsl:value-of>
                          </td>
                        </tr>
                        <tr>
                          <td style="text-align: left; vertical-align: top; width: 49px; height: 25px;">
                            Release#: <xsl:value-of select="OrderMasterDTO/ReleaseNumber"></xsl:value-of>
                          </td>
                          <td>
                            Status: <xsl:value-of select="OrderMasterDTO/OrderStatus"></xsl:value-of>
                          </td>
                        </tr>
                        <tr>
                          <td style="text-align: left; vertical-align: top; width: 49px; height: 25px;">
                            Supplier: <xsl:value-of select="OrderMasterDTO/SupplierName"></xsl:value-of>
                          </td>
                          <td>
                            Order Cost: <xsl:value-of select="OrderMasterDTO/OrderCost"></xsl:value-of>
                          </td>
                        </tr>
                      </tbody>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td style="width: 99%; height: 25px;">
                  </td>
                </tr>
                <tr>
                  <td style="width: 99%; text-align: left; vertical-align: top; height: 25px;">

                    <table align="left" border="1" cellpadding="1" cellspacing="1" style="width: 99%; height: 25px">
                      <thead>
                        <tr>
                          <th style="width: 16%; text-align: right;">
                            ItemNumber
                          </th>
                          <th style="width: 16%; text-align: right;">
                            Location
                          </th>
                          <th style="width: 16%; text-align: right;">
                            Requested Quantity
                          </th>
                          <th style="width: 16%; text-align: right;">
                            Approved Quantity
                          </th>
                          <th style="width: 16%; text-align: right;">
                            Required Date
                          </th>
                          <th style="width: 16%; text-align: right;">
                            Received Qantity
                          </th>
                        </tr>
                      </thead>
                      <tbody>
                        <xsl:for-each select="OrderMasterDTO/OrderListItem/OrderDetailsDTO">
                          <tr>
                            <td style="width: 16%; text-align: right;">
                              <xsl:value-of select="ItemNumber"></xsl:value-of>
                            </td>
                            <td style="width: 16%; text-align: right;">
                              <xsl:value-of select="Bin"></xsl:value-of>
                            </td>
                            <td style="width: 16%; text-align: right;">
                              <xsl:value-of select="RequestedQuantity"></xsl:value-of>
                            </td>
                            <td style="width: 16%; text-align: right;">
                              <xsl:value-of select="ApprovedQuantity"></xsl:value-of>
                            </td>
                            <td style="width: 16%; text-align: right;">
                              <xsl:value-of select="RequiredDate"></xsl:value-of>
                            </td>
                            <td style="width: 16%; text-align: right;">
                              <xsl:value-of select="ReceivedQuantity"></xsl:value-of>
                            </td>
                          </tr>
                        </xsl:for-each>
                      </tbody>
                      <tfoot>
                      </tfoot>
                    </table>
                  </td>
                </tr>
              </tbody>
              <tfoot>
                <tr>
                  <td style="text-align: left; vertical-align: middle; width: 99%; height: 22px;">
                    <p style="font-size: 12px; color: #000; line-height: 18px; margin: 0; padding: 0 0 25px 0;">
                      Congratulations again!
                    </p>
                  </td>
                </tr>
              </tfoot>
            </table>
          </td>
        </tr>
      </tbody>
      <tfoot>
        <tr>
          <td align="center" style="font-size: 11px; color: #8f8f8f; height: 25px;" valign="bottom">
            Copyright @ <xsl:value-of select="$copyrightyear"></xsl:value-of> <a href="#" style="color: #8f8f8f; text-decoration: none;" title="eTurns">eTurns.com.</a>All rights reserved.
          </td>
        </tr>
      </tfoot>
    </table>
  </xsl:template>
</xsl:stylesheet>
