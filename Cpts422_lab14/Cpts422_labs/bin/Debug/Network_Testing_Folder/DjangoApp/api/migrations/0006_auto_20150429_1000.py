# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models, migrations


class Migration(migrations.Migration):

    dependencies = [
        ('api', '0005_auto_20150429_0957'),
    ]

    operations = [
        migrations.AlterField(
            model_name='api',
            name='pub_date',
            field=models.DateTimeField(auto_now_add=True, verbose_name=b'date published'),
            preserve_default=True,
        ),
    ]
